import {
	ChangeDetectionStrategy,
	ChangeDetectorRef,
	Component,
	DestroyRef,
	inject,
	OnInit,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';
import { GmButtonComponent, GmCardComponent } from '../../shared';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { CourseMaterialService } from '../../page-agency/course-material.service';
import { Course } from '../../core/models/course.model';
import { Lecture, LectureMaterial } from '../../core/models/lecture.model';
import { STORAGE_KEYS } from '../../utilities/storage-keys.const';

@Component({
	selector: 'app-course-learn',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, GmButtonComponent, GmCardComponent],
	templateUrl: './course-learn.component.html',
	styleUrl: './course-learn.component.scss',
})
export class CourseLearnComponent implements OnInit {
	courseId = '';
	course: Course | null = null;

	lectures: Lecture[] = [];
	selectedLecture: Lecture | null = null;
	materials: LectureMaterial[] = [];

	// The backend payment-gates lecture access and returns 403 for students
	// who have not paid; we surface that as a friendly locked state.
	accessDenied = false;

	// Completion is tracked client-side only — the API has no progress model
	// yet — so it is an honest local convenience, not authoritative data.
	private completedLectureIds = new Set<string>();

	loader = inject(LoaderModalStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private route = inject(ActivatedRoute);
	private router = inject(Router);
	private courseService = inject(CourseAgencyService);
	private courseMaterialService = inject(CourseMaterialService);

	ngOnInit() {
		this.route.params.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: params => {
				this.courseId = params['courseId'];
				this.completedLectureIds = this.readCompleted(this.courseId);
				this.loadCourse();
				this.loadLectures();
			},
		});
	}

	private loadCourse() {
		this.courseService
			.getCourse(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.course = res;
					this.cdr.markForCheck();
				},
				error: () => {},
			});
	}

	private loadLectures() {
		patchState(this.loader, { isShow: true });
		this.courseMaterialService
			.getLecturesByCourse(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.lectures = res.sort((a, b) => (a.order ?? 0) - (b.order ?? 0));
					if (this.lectures.length > 0) {
						this.selectLecture(this.lectures[0]);
					}
					patchState(this.loader, { isShow: false });
					this.cdr.markForCheck();
				},
				error: err => {
					patchState(this.loader, { isShow: false });
					if (err.status === 403) {
						this.accessDenied = true;
					} else {
						this.toastService.show('Failed to load course content', 'error');
					}
					this.cdr.markForCheck();
				},
			});
	}

	selectLecture(lecture: Lecture) {
		this.selectedLecture = lecture;
		this.materials = [];
		patchState(this.loader, { isShow: true });
		this.courseMaterialService
			.getMaterialsByLecture(lecture.lectureId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.materials = res;
					patchState(this.loader, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
					this.toastService.show('Failed to load materials', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	// Normalised, case-insensitive type used by the template's @switch.
	materialType(material: LectureMaterial): string {
		return (material.materialType ?? '').trim().toUpperCase();
	}

	// ---------- Local completion tracking ----------
	isCompleted(lectureId: string): boolean {
		return this.completedLectureIds.has(lectureId);
	}

	toggleCompleted(lecture: Lecture) {
		if (this.completedLectureIds.has(lecture.lectureId)) {
			this.completedLectureIds.delete(lecture.lectureId);
		} else {
			this.completedLectureIds.add(lecture.lectureId);
		}
		this.writeCompleted(this.courseId, this.completedLectureIds);
		this.cdr.markForCheck();
	}

	get completedCount(): number {
		return this.lectures.filter(l => this.completedLectureIds.has(l.lectureId)).length;
	}

	get progressPercent(): number {
		if (this.lectures.length === 0) return 0;
		return Math.round((this.completedCount / this.lectures.length) * 100);
	}

	goToDetails() {
		this.router.navigate(['/client/course-details', this.courseId]);
	}

	private storageKey(): string {
		return STORAGE_KEYS.completedLectures;
	}

	private readCompleted(courseId: string): Set<string> {
		try {
			const raw = localStorage.getItem(this.storageKey());
			const map: Record<string, string[]> = raw ? JSON.parse(raw) : {};
			return new Set(map[courseId] ?? []);
		} catch {
			return new Set<string>();
		}
	}

	private writeCompleted(courseId: string, ids: Set<string>) {
		try {
			const raw = localStorage.getItem(this.storageKey());
			const map: Record<string, string[]> = raw ? JSON.parse(raw) : {};
			map[courseId] = Array.from(ids);
			localStorage.setItem(this.storageKey(), JSON.stringify(map));
		} catch {
			// localStorage may be unavailable (private mode); completion is
			// non-critical, so silently skip persistence.
		}
	}
}

import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { CourseAgencyService } from '../course-agency.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { UserStore } from '../../stores/user.store';
import { ToastService } from '../../shared/services/toast.service';
import { Course } from '../../core/models/course.model';
import { DEFAULT_COURSE_THUMBNAIL_URL } from '../../utilities/defaults.const';
import { FallbackSrcDirective } from '../../shared/directives/fallback-src.directive';
import { matchesSearch } from '../../shared/utils/search.util';

@Component({
    selector: 'app-manage-courses',
    standalone: true,
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [CommonModule, RouterLink, FormsModule, DecimalPipe, FallbackSrcDirective],
    templateUrl: './manage-courses.component.html'
})
export class ManageCoursesComponent implements OnInit {
	courses: Course[] = [];
	filteredCourses: Course[] = [];
	
	searchString = '';
	selectedCategory = '';
	readonly DEFAULT_COURSE_THUMBNAIL_URL = DEFAULT_COURSE_THUMBNAIL_URL;
	
	isShowDeleteModal = false;
	courseIdToDelete = '';
	
	loaderStore = inject(LoaderModalStore);
	userStore = inject(UserStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(private courseAgencyService: CourseAgencyService) {}

	ngOnInit() {
		this.loadCourses();
	}

	loadCourses() {
		patchState(this.loaderStore, { isShow: true });
		this.courseAgencyService
			.getCoursesOfTrainer(this.userStore.trainerId() ?? '')
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.courses = res;
					this.applyFilters();
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load courses', 'error');
				},
			});
	}

	applyFilters() {
		let result = this.courses;
		if (this.searchString.trim()) {
			result = result.filter(c => matchesSearch(this.searchString, [c.title, c.description]));
		}
		if (this.selectedCategory) {
			result = result.filter(c => c.category === this.selectedCategory);
		}
		this.filteredCourses = result;
		this.cdr.markForCheck();
	}

	onSearch() {
		this.applyFilters();
	}

	onCategoryChange(cat: string) {
		this.selectedCategory = cat;
		this.applyFilters();
	}

	getCourseThumbnail(course: Course): string {
		return course.getFileDtos?.find(file => file.typeFile === 'IMAGE')?.url || DEFAULT_COURSE_THUMBNAIL_URL;
	}

	onShowDeleteModel(flag: boolean, id: string) {
		this.isShowDeleteModal = flag;
		this.courseIdToDelete = id;
	}

	onRemove() {
		this.isShowDeleteModal = false;
		patchState(this.loaderStore, { isShow: true });

		this.courseAgencyService
			.removeCourse(this.courseIdToDelete)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Course removed successfully');
					this.courses = this.courses.filter(x => x.courseId !== this.courseIdToDelete);
					this.courseIdToDelete = '';
					this.applyFilters();
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to remove course', 'error');
				},
			});
	}

	get categories(): string[] {
		const cats = this.courses.map(c => c.category).filter(Boolean);
		return Array.from(new Set(cats));
	}

	get totalSeats(): number {
		return this.courses.reduce((sum, c) => sum + (c.maxParticipants || 0), 0);
	}

	get avgPrice(): number {
		if (this.courses.length === 0) {
			return 0;
		}
		return this.courses.reduce((sum, c) => sum + (c.price || 0), 0) / this.courses.length;
	}

	private readonly typeIcons: Record<string, string> = {
		'Yoga': '🧘',
		'Cardio': '🏃',
		'Strength': '🏋️',
		'Pilates': '🤸',
		'Stretching': '🙆',
		'Cross fit': '⚡',
	};

	typeIcon(type: string): string {
		return this.typeIcons[type] ?? '🏋️';
	}
}

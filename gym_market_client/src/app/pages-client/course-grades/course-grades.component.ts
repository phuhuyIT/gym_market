import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { GradebookService } from '../../core/services/gradebook.service';
import { CategoryGrade, GradeItemScore, MyCourseGrades } from '../../core/models/gradebook.model';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-course-grades',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, RouterLink, DecimalPipe],
	templateUrl: './course-grades.component.html',
	styleUrl: './course-grades.component.scss',
})
export class CourseGradesComponent implements OnInit {
	courseId = '';
	grades: MyCourseGrades | null = null;
	isLoading = false;

	private route = inject(ActivatedRoute);
	private destroyRef = inject(DestroyRef);
	private gradebookService = inject(GradebookService);
	private toastService = inject(ToastService);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit() {
		this.courseId = this.route.snapshot.params['courseId'];
		this.load();
	}

	load() {
		if (!this.courseId) return;
		this.isLoading = true;
		this.gradebookService
			.getMyGrades(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: grades => {
					this.grades = grades;
					this.isLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load grades', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	gradeClass(percent?: number | null): string {
		if (percent === null || percent === undefined) return 'muted';
		if (percent >= 90) return 'a';
		if (percent >= 80) return 'b';
		if (percent >= 70) return 'c';
		if (percent >= 60) return 'd';
		return 'f';
	}

	categoryProgress(category: CategoryGrade): number {
		return Math.max(0, Math.min(100, category.finalPercent || 0));
	}

	scoreText(item: GradeItemScore): string {
		if (item.scorePercent === null || item.scorePercent === undefined) return 'Missing';
		return `${item.scorePercent.toFixed(1)}%`;
	}

	get gradedItemCount(): number {
		return this.grades?.grade.items.filter(item => item.scorePercent !== null && item.scorePercent !== undefined).length ?? 0;
	}
}

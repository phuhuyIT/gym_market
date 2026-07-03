import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MyCourseAnalytics } from '../../core/models/course-analytics.model';
import { CourseAnalyticsService } from '../../core/services/course-analytics.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-course-progress-dashboard',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, RouterLink, DecimalPipe, DatePipe],
	templateUrl: './course-progress-dashboard.component.html',
	styleUrl: './course-progress-dashboard.component.scss',
})
export class CourseProgressDashboardComponent implements OnInit {
	courseId = '';
	analytics: MyCourseAnalytics | null = null;
	isLoading = false;

	private route = inject(ActivatedRoute);
	private destroyRef = inject(DestroyRef);
	private analyticsService = inject(CourseAnalyticsService);
	private toastService = inject(ToastService);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit() {
		this.courseId = this.route.snapshot.params['courseId'];
		this.load();
	}

	load() {
		if (!this.courseId) return;
		this.isLoading = true;
		this.analyticsService
			.getMyCourseProgress(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: analytics => {
					this.analytics = analytics;
					this.isLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load course progress', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	gradeClass(percent?: number | null): string {
		if (percent === null || percent === undefined) return 'muted';
		if (percent >= 80) return 'strong';
		if (percent >= 60) return 'steady';
		return 'risk';
	}

	progressWidth(percent: number): string {
		return `${Math.max(0, Math.min(100, percent))}%`;
	}
}

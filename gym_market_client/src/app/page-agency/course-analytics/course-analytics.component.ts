import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CourseAnalyticsDashboard, CourseLearnerAnalytics } from '../../core/models/course-analytics.model';
import { CourseAnalyticsService } from '../../core/services/course-analytics.service';
import { ToastService } from '../../shared/services/toast.service';

type LearnerFilter = 'all' | 'atRisk' | 'complete';

@Component({
	selector: 'app-course-analytics',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, RouterLink, DecimalPipe, DatePipe],
	templateUrl: './course-analytics.component.html',
	styleUrl: './course-analytics.component.scss',
})
export class CourseAnalyticsComponent implements OnInit {
	courseId = '';
	dashboard: CourseAnalyticsDashboard | null = null;
	isLoading = false;
	filter: LearnerFilter = 'all';

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
			.getCourseDashboard(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: dashboard => {
					this.dashboard = dashboard;
					this.isLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load course analytics', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	setFilter(filter: LearnerFilter) {
		this.filter = filter;
	}

	gradeClass(percent?: number | null): string {
		if (percent === null || percent === undefined) return 'muted';
		if (percent >= 80) return 'strong';
		if (percent >= 60) return 'steady';
		return 'risk';
	}

	progressStyle(percent: number): string {
		return `${Math.max(0, Math.min(100, percent))}%`;
	}

	trendHeight(value: number): string {
		const max = this.maxTrendValue;
		if (max <= 0) return '0%';
		return `${Math.max(8, Math.round((value / max) * 100))}%`;
	}

	riskClass(score: number): string {
		if (score >= 70) return 'risk-high';
		if (score >= 40) return 'risk-medium';
		return 'risk-low';
	}

	get maxTrendValue(): number {
		const values = this.dashboard?.trends.flatMap(trend => [
			trend.completedLessons,
			trend.assignmentSubmissions,
			trend.quizAttempts,
			trend.discussionPosts,
		]) ?? [];
		return Math.max(0, ...values);
	}

	get topRiskLearners(): CourseLearnerAnalytics[] {
		return [...(this.dashboard?.learners ?? [])]
			.filter(learner => learner.isAtRisk)
			.sort((a, b) => b.riskScore - a.riskScore)
			.slice(0, 4);
	}

	get performanceItems() {
		return [...(this.dashboard?.performanceItems ?? [])]
			.sort((a, b) => (a.averagePercent ?? -1) - (b.averagePercent ?? -1));
	}

	get filteredLearners(): CourseLearnerAnalytics[] {
		const learners = this.dashboard?.learners ?? [];
		if (this.filter === 'atRisk') return learners.filter(learner => learner.isAtRisk);
		if (this.filter === 'complete') return learners.filter(learner => learner.isCompleted);
		return learners;
	}
}

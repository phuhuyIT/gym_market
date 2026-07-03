import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { GradebookService } from '../../core/services/gradebook.service';
import {
	CourseGradebook,
	GradeCategory,
	GradeItem,
	StudentGradeSummary,
	UpdateGradeCategory,
} from '../../core/models/gradebook.model';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-gradebook',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, FormsModule, RouterLink, DecimalPipe],
	templateUrl: './gradebook.component.html',
	styleUrl: './gradebook.component.scss',
})
export class GradebookComponent implements OnInit {
	courseId = '';
	gradebook: CourseGradebook | null = null;
	categoryDrafts: UpdateGradeCategory[] = [];
	itemCategory: Record<string, string | null> = {};
	isLoading = false;
	isSaving = false;
	isExporting = false;

	private destroyRef = inject(DestroyRef);
	private route = inject(ActivatedRoute);
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
			.getCourseGradebook(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: gradebook => {
					this.gradebook = gradebook;
					this.categoryDrafts = gradebook.categories.map(category => ({ ...category }));
					this.itemCategory = Object.fromEntries(gradebook.items.map(item => [item.itemId, item.categoryId ?? null]));
					this.isLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load gradebook', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	addCategory() {
		this.categoryDrafts = [
			...this.categoryDrafts,
			{
				categoryId: `tmp-${Date.now()}`,
				name: 'New category',
				weightPercent: 0,
				order: this.categoryDrafts.length + 1,
			},
		];
	}

	removeCategory(categoryId?: string | null) {
		if (this.categoryDrafts.length <= 1 || !categoryId) return;
		this.categoryDrafts = this.categoryDrafts
			.filter(category => category.categoryId !== categoryId)
			.map((category, index) => ({ ...category, order: index + 1 }));
		const fallbackId = this.categoryDrafts[0]?.categoryId ?? null;
		for (const itemId of Object.keys(this.itemCategory)) {
			if (this.itemCategory[itemId] === categoryId) {
				this.itemCategory[itemId] = fallbackId;
			}
		}
	}

	savePolicy() {
		if (!this.gradebook || this.isSaving) return;
		if (Math.abs(this.totalWeight - 100) > 0.01) {
			this.toastService.show('Category weights must total 100%', 'error');
			return;
		}

		this.isSaving = true;
		this.gradebookService
			.updatePolicy(this.courseId, {
				categories: this.categoryDrafts.map((category, index) => ({
					categoryId: category.categoryId,
					name: category.name,
					weightPercent: Number(category.weightPercent) || 0,
					order: index + 1,
				})),
				items: this.gradebook.items.map(item => ({
					itemId: item.itemId,
					categoryId: this.itemCategory[item.itemId],
				})),
			})
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.isSaving = false;
					this.toastService.show('Gradebook policy saved');
					this.load();
				},
				error: err => {
					this.isSaving = false;
					const message = err?.error?.message || err?.error?.Message || 'Failed to save gradebook policy';
					this.toastService.show(message, 'error');
					this.cdr.markForCheck();
				},
			});
	}

	exportCsv() {
		if (this.isExporting) return;
		this.isExporting = true;
		this.gradebookService
			.exportCourseGradebook(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: blob => {
					this.isExporting = false;
					const url = URL.createObjectURL(blob);
					const link = document.createElement('a');
					link.href = url;
					link.download = `gradebook-${this.courseId}.csv`;
					link.click();
					URL.revokeObjectURL(url);
					this.cdr.markForCheck();
				},
				error: () => {
					this.isExporting = false;
					this.toastService.show('Failed to export gradebook', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	scoreFor(student: StudentGradeSummary, item: GradeItem): number | null {
		return student.items.find(score => score.itemId === item.itemId)?.scorePercent ?? null;
	}

	categoryName(categoryId?: string | null): string {
		return this.gradebook?.categories.find(category => category.categoryId === categoryId)?.name ?? 'Unassigned';
	}

	gradeClass(percent?: number | null): string {
		if (percent === null || percent === undefined) return 'gradebook-muted';
		if (percent >= 90) return 'gradebook-a';
		if (percent >= 80) return 'gradebook-b';
		if (percent >= 70) return 'gradebook-c';
		if (percent >= 60) return 'gradebook-d';
		return 'gradebook-f';
	}

	get totalWeight(): number {
		return this.categoryDrafts.reduce((sum, category) => sum + (Number(category.weightPercent) || 0), 0);
	}

	get sortedItems(): GradeItem[] {
		return [...(this.gradebook?.items ?? [])].sort((a, b) => this.categoryName(a.categoryId).localeCompare(this.categoryName(b.categoryId)));
	}

	get missingCount(): number {
		return this.gradebook?.students.reduce((sum, student) => sum + student.items.filter(item => item.scorePercent === null || item.scorePercent === undefined).length, 0) ?? 0;
	}
}

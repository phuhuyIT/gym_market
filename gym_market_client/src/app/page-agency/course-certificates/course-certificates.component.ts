import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CourseMaterialService } from '../course-material.service';
import { CourseCertificateSetting, UpdateCourseCertificateSetting } from '../../core/models/certificate.model';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-course-certificates',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, FormsModule, RouterLink, DecimalPipe],
	templateUrl: './course-certificates.component.html',
	styleUrl: './course-certificates.component.scss',
})
export class CourseCertificatesComponent implements OnInit {
	courseId = '';
	setting: CourseCertificateSetting | null = null;
	draft: UpdateCourseCertificateSetting = this.defaultDraft();
	isLoading = false;
	isSaving = false;

	private route = inject(ActivatedRoute);
	private destroyRef = inject(DestroyRef);
	private courseMaterialService = inject(CourseMaterialService);
	private toastService = inject(ToastService);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit() {
		this.courseId = this.route.snapshot.params['courseId'];
		this.load();
	}

	load() {
		if (!this.courseId) return;
		this.isLoading = true;
		this.courseMaterialService
			.getCertificateSettings(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: setting => {
					this.setting = setting;
					this.draft = {
						isEnabled: setting.isEnabled,
						templateName: setting.templateName,
						certificateTitle: setting.certificateTitle,
						bodyText: setting.bodyText,
						accentColor: setting.accentColor || '#007AFF',
						requiredLecturePercent: setting.requiredLecturePercent,
						requirePublishedQuizzes: setting.requirePublishedQuizzes,
						requirePublishedAssignments: setting.requirePublishedAssignments,
						requiredAssignmentPercent: setting.requiredAssignmentPercent,
						minimumFinalGradePercent: setting.minimumFinalGradePercent,
					};
					this.isLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load certificate settings', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	save() {
		if (this.isSaving) return;
		if (!this.draft.templateName.trim() || !this.draft.certificateTitle.trim() || !this.draft.bodyText.trim()) {
			this.toastService.show('Template name, title, and body text are required', 'error');
			return;
		}
		if (!this.percentIsValid(this.draft.requiredLecturePercent)
			|| !this.percentIsValid(this.draft.requiredAssignmentPercent)
			|| !this.percentIsValid(this.draft.minimumFinalGradePercent)) {
			this.toastService.show('Requirement percentages must be between 0 and 100', 'error');
			return;
		}

		this.isSaving = true;
		const payload: UpdateCourseCertificateSetting = {
			...this.draft,
			templateName: this.draft.templateName.trim(),
			certificateTitle: this.draft.certificateTitle.trim(),
			bodyText: this.draft.bodyText.trim(),
			accentColor: this.draft.accentColor || '#007AFF',
			requiredLecturePercent: Number(this.draft.requiredLecturePercent) || 0,
			requiredAssignmentPercent: Number(this.draft.requiredAssignmentPercent) || 0,
			minimumFinalGradePercent: this.draft.minimumFinalGradePercent === null || this.draft.minimumFinalGradePercent === undefined
				? null
				: Number(this.draft.minimumFinalGradePercent),
		};

		this.courseMaterialService
			.updateCertificateSettings(this.courseId, payload)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: setting => {
					this.setting = setting;
					this.isSaving = false;
					this.toastService.show('Certificate settings saved');
					this.cdr.markForCheck();
				},
				error: err => {
					this.isSaving = false;
					const message = err?.error?.message || err?.error?.Message || 'Failed to save certificate settings';
					this.toastService.show(message, 'error');
					this.cdr.markForCheck();
				},
			});
	}

	resetDefaults() {
		this.draft = this.defaultDraft();
		this.cdr.markForCheck();
	}

	private defaultDraft(): UpdateCourseCertificateSetting {
		return {
			isEnabled: true,
			templateName: 'Classic',
			certificateTitle: 'Certificate of Completion',
			bodyText: 'Awarded for successfully completing this course.',
			accentColor: '#007AFF',
			requiredLecturePercent: 100,
			requirePublishedQuizzes: true,
			requirePublishedAssignments: false,
			requiredAssignmentPercent: 0,
			minimumFinalGradePercent: null,
		};
	}

	private percentIsValid(value?: number | null): boolean {
		if (value === null || value === undefined) return true;
		const numeric = Number(value);
		return Number.isFinite(numeric) && numeric >= 0 && numeric <= 100;
	}
}

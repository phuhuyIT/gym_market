import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CourseMaterialService } from '../../page-agency/course-material.service';
import { CourseCertificate } from '../../core/models/certificate.model';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-client-course-certificates',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, RouterLink, DatePipe],
	templateUrl: './course-certificates.component.html',
	styleUrl: './course-certificates.component.scss',
})
export class CourseCertificatesComponent implements OnInit {
	certificates: CourseCertificate[] = [];
	isLoading = false;

	private destroyRef = inject(DestroyRef);
	private courseMaterialService = inject(CourseMaterialService);
	private toastService = inject(ToastService);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit() {
		this.load();
	}

	load() {
		this.isLoading = true;
		this.courseMaterialService
			.getMyCertificates()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: certificates => {
					this.certificates = certificates;
					this.isLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load certificates', 'error');
					this.cdr.markForCheck();
				},
			});
	}
}

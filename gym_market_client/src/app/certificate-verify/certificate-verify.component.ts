import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CourseMaterialService } from '../page-agency/course-material.service';
import { CourseCertificate } from '../core/models/certificate.model';

@Component({
	selector: 'app-certificate-verify',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, RouterLink, DatePipe],
	templateUrl: './certificate-verify.component.html',
	styleUrl: './certificate-verify.component.scss',
})
export class CertificateVerifyComponent implements OnInit {
	verificationCode = '';
	certificate: CourseCertificate | null = null;
	isLoading = false;
	notFound = false;

	private route = inject(ActivatedRoute);
	private destroyRef = inject(DestroyRef);
	private courseMaterialService = inject(CourseMaterialService);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit() {
		this.verificationCode = this.route.snapshot.params['verificationCode'];
		this.verify();
	}

	verify() {
		if (!this.verificationCode) return;
		this.isLoading = true;
		this.notFound = false;
		this.courseMaterialService
			.verifyCertificate(this.verificationCode)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: certificate => {
					this.certificate = certificate;
					this.isLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.certificate = null;
					this.notFound = true;
					this.isLoading = false;
					this.cdr.markForCheck();
				},
			});
	}
}

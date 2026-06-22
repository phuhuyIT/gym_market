import {
	ChangeDetectionStrategy,
	ChangeDetectorRef,
	Component,
	DestroyRef,
	inject,
	OnInit,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, DecimalPipe } from '@angular/common';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';
import { GmButtonComponent, GmCardComponent } from '../../shared';
import { CourseRegistrationService } from '../course-registration.service';
import { CoursePaymentInfo } from '../../core/models/course-registration.model';

@Component({
	selector: 'app-course-payment',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, DecimalPipe, GmButtonComponent, GmCardComponent],
	templateUrl: './course-payment.component.html',
	styleUrl: './course-payment.component.scss',
})
export class CoursePaymentComponent implements OnInit {
	courseId = '';
	info: CoursePaymentInfo | null = null;

	loader = inject(LoaderModalStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private route = inject(ActivatedRoute);
	private router = inject(Router);
	private registrationService = inject(CourseRegistrationService);

	ngOnInit() {
		this.route.params.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: params => {
				this.courseId = params['courseId'];
				this.loadPaymentInfo();
			},
		});
	}

	private loadPaymentInfo() {
		patchState(this.loader, { isShow: true });
		this.registrationService
			.getPaymentInfo(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.info = res;
					patchState(this.loader, { isShow: false });
					this.cdr.markForCheck();
				},
				error: err => {
					patchState(this.loader, { isShow: false });
					if (err.status === 404) {
						// No registration yet — send them back to enrol first.
						this.toastService.show('Please enrol in this course first.', 'error');
						this.router.navigate(['/client/course-details', this.courseId]);
					} else {
						this.toastService.show('Failed to load payment details', 'error');
					}
					this.cdr.markForCheck();
				},
			});
	}

	get isPaid(): boolean {
		return this.info?.status === 'Paid';
	}

	get isCanceled(): boolean {
		return this.info?.status === 'Canceled';
	}

	// VietQR image with the amount and transfer reference pre-filled. The student
	// scans it with any Vietnamese banking app; the trainer confirms the transfer
	// manually and approves the payment.
	get qrUrl(): string {
		const i = this.info;
		if (!i || !i.bankConfigured || !i.bankBin || !i.bankAccountNo) {
			return '';
		}
		const amount = Math.round(i.amount || 0);
		const addInfo = encodeURIComponent(i.reference || '');
		const accountName = encodeURIComponent(i.bankAccountName || '');
		return `https://img.vietqr.io/image/${i.bankBin}-${i.bankAccountNo}-compact2.png?amount=${amount}&addInfo=${addInfo}&accountName=${accountName}`;
	}

	// Student confirms they've transferred the money: notify the trainer to verify,
	// and refresh status in case the trainer has already approved it.
	confirmPayment() {
		patchState(this.loader, { isShow: true });
		this.registrationService
			.confirmPayment(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.info = res;
					patchState(this.loader, { isShow: false });
					if (res.status === 'Paid') {
						this.toastService.show('Payment confirmed! Your course is unlocked.');
						this.router.navigate(['/client/course-learn', this.courseId]);
					} else {
						const who = res.trainerName || 'your trainer';
						this.toastService.show(
							`Thanks! We've notified ${who} to confirm your transfer.`
						);
					}
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
					this.toastService.show('Failed to notify your trainer. Please try again.', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	payWithMomo() {
		patchState(this.loader, { isShow: true });
		this.registrationService
			.createMomoPayment(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					patchState(this.loader, { isShow: false });
					if (res?.payUrl) {
						window.location.href = res.payUrl;
					} else {
						this.toastService.show('Momo payment is not available for this course.', 'error');
					}
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
					this.toastService.show('Failed to start Momo payment. Please try again.', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	retryPayment() {
		patchState(this.loader, { isShow: true });
		this.registrationService
			.registerCourse(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loader, { isShow: false });
					this.toastService.show('Payment restarted. Please complete the new payment.');
					this.loadPaymentInfo();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
					this.toastService.show('Could not restart payment for this course.', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	copy(value: string | number | undefined) {
		if (value === undefined || value === null || value === '') return;
		navigator.clipboard?.writeText(String(value)).then(
			() => this.toastService.show('Copied to clipboard'),
			() => {}
		);
	}

	goToContent() {
		this.router.navigate(['/client/course-learn', this.courseId]);
	}

	goToDetails() {
		this.router.navigate(['/client/course-details', this.courseId]);
	}
}

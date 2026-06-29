import { Component, DestroyRef, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../guest/account.service';
import { ALLOWED_IMAGE_TYPES, MAX_AVATAR_BYTES } from '../utilities/upload.const';
import { UserStore } from '../stores/user.store';
import { ToastService } from '../shared/services/toast.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { GmButtonComponent } from '../shared';
import { patchState } from '@ngrx/signals';
import { DEFAULT_AVATAR_URL } from '../utilities/defaults.const';
import { NotificationPreferencesComponent } from '../shared/components/notification-preferences/notification-preferences.component';

@Component({
	selector: 'app-account-settings',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [FormsModule, GmButtonComponent, NotificationPreferencesComponent],
	templateUrl: './account-settings.component.html',
	styleUrl: './account-settings.component.scss',
})
export class AccountSettingsComponent {
	private accountService = inject(AccountService);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	userStore = inject(UserStore);

	readonly DEFAULT_AVATAR_URL = DEFAULT_AVATAR_URL;

	// Avatar
	avatarPreview = signal<string>(this.userStore.avatar() || DEFAULT_AVATAR_URL);
	selectedFile: File | null = null;
	avatarLoading = signal(false);

	// Change Password
	passwordModel = { currentPassword: '', newPassword: '', confirmNewPassword: '' };
	passwordLoading = signal(false);

	// Email Confirmation
	emailConfirmLoading = signal(false);
	emailConfirmSent = signal(false);

	// 2FA
	twoFALoading = signal(false);
	twoFAEnabled = signal(false);
	twoFASetup = signal<{ sharedKey: string; qrCodeUri: string } | null>(null);
	verifyCode = '';
	verifyLoading = signal(false);

	// ── Avatar Upload ─────────────────────────────────────

	onFileSelected(event: Event) {
		const input = event.target as HTMLInputElement;
		if (!input.files?.length) return;

		const file = input.files[0];
		if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
			this.toastService.show('Only JPEG, PNG, WebP, and GIF images are allowed', 'error');
			return;
		}
		if (file.size > MAX_AVATAR_BYTES) {
			this.toastService.show('File must be under 5 MB', 'error');
			return;
		}

		this.selectedFile = file;
		const reader = new FileReader();
		reader.onload = () => this.avatarPreview.set(reader.result as string);
		reader.readAsDataURL(file);
	}

	uploadAvatar() {
		if (!this.selectedFile) return;
		this.avatarLoading.set(true);

		this.accountService
			.uploadAvatar(this.selectedFile)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.avatarLoading.set(false);
					if (res.success) {
						patchState(this.userStore, { avatar: res.avatarUrl });
						this.avatarPreview.set(res.avatarUrl);
						this.selectedFile = null;
						this.toastService.show('Avatar updated', 'success');
					}
				},
				error: err => {
					this.avatarLoading.set(false);
					this.toastService.show(err.error?.errors?.[0] || 'Upload failed', 'error');
				},
			});
	}

	// ── Change Password ───────────────────────────────────

	onChangePassword() {
		const { currentPassword, newPassword, confirmNewPassword } = this.passwordModel;
		if (!currentPassword || !newPassword || !confirmNewPassword) {
			this.toastService.show('Please fill in all fields', 'error');
			return;
		}
		if (newPassword !== confirmNewPassword) {
			this.toastService.show('New passwords do not match', 'error');
			return;
		}
		if (newPassword.length < 8 || newPassword.length > 16) {
			this.toastService.show('Password must be 8-16 characters', 'error');
			return;
		}

		this.passwordLoading.set(true);
		this.accountService
			.changePassword(this.passwordModel)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.passwordLoading.set(false);
					if (res.success) {
						this.passwordModel = { currentPassword: '', newPassword: '', confirmNewPassword: '' };
						this.toastService.show('Password changed successfully', 'success');
					}
				},
				error: err => {
					this.passwordLoading.set(false);
					this.toastService.show(err.error?.errors?.[0] || 'Failed to change password', 'error');
				},
			});
	}

	// ── Email Confirmation ────────────────────────────────

	onSendEmailConfirmation() {
		this.emailConfirmLoading.set(true);
		this.accountService
			.sendEmailConfirmation()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.emailConfirmLoading.set(false);
					if (res.success) {
						this.emailConfirmSent.set(true);
						this.toastService.show('Confirmation email sent', 'success');
					}
				},
				error: err => {
					this.emailConfirmLoading.set(false);
					const msg = err.error?.errors?.[0];
					if (msg === 'EMAIL_ALREADY_CONFIRMED') {
						this.toastService.show('Email is already confirmed', 'success');
					} else {
						this.toastService.show(msg || 'Failed to send confirmation', 'error');
					}
				},
			});
	}

	// ── Two-Factor Authentication ─────────────────────────

	onEnable2FA() {
		this.twoFALoading.set(true);
		this.accountService
			.enable2FA()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.twoFALoading.set(false);
					if (res.success) {
						this.twoFASetup.set({ sharedKey: res.sharedKey, qrCodeUri: res.qrCodeUri });
					}
				},
				error: err => {
					this.twoFALoading.set(false);
					this.toastService.show(err.error?.errors?.[0] || 'Failed to enable 2FA', 'error');
				},
			});
	}

	onVerify2FA() {
		if (!this.verifyCode || this.verifyCode.length !== 6) {
			this.toastService.show('Please enter a 6-digit code', 'error');
			return;
		}
		this.verifyLoading.set(true);
		this.accountService
			.verify2FA(this.verifyCode)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.verifyLoading.set(false);
					if (res.success) {
						this.twoFAEnabled.set(true);
						this.twoFASetup.set(null);
						this.verifyCode = '';
						this.toastService.show('Two-factor authentication enabled', 'success');
					}
				},
				error: err => {
					this.verifyLoading.set(false);
					this.toastService.show(err.error?.errors?.[0] || 'Invalid code', 'error');
				},
			});
	}

	onDisable2FA() {
		this.twoFALoading.set(true);
		this.accountService
			.disable2FA()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.twoFALoading.set(false);
					if (res.success) {
						this.twoFAEnabled.set(false);
						this.toastService.show('Two-factor authentication disabled', 'success');
					}
				},
				error: err => {
					this.twoFALoading.set(false);
					this.toastService.show(err.error?.errors?.[0] || 'Failed to disable 2FA', 'error');
				},
			});
	}

	getQrImageUrl(uri: string): string {
		return `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(uri)}`;
	}
}

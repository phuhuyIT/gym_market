import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { type NotificationEmailFrequency, type NotificationPreference } from '../../../core/models/notification.model';
import { NotificationService } from '../../../core/services/notification.service';
import { ToastService } from '../../services/toast.service';

@Component({
	selector: 'app-notification-preferences',
	changeDetection: ChangeDetectionStrategy.OnPush,
	templateUrl: './notification-preferences.component.html',
	styleUrl: './notification-preferences.component.scss',
})
export class NotificationPreferencesComponent implements OnInit {
	private readonly notificationService = inject(NotificationService);
	private readonly toastService = inject(ToastService);

	readonly preferences = signal<NotificationPreference[]>([]);
	readonly originalPreferences = signal<NotificationPreference[]>([]);
	readonly loading = signal(false);
	readonly saving = signal(false);

	readonly inAppEnabledCount = computed(() => this.preferences().filter(preference => preference.inAppEnabled).length);
	readonly emailEnabledCount = computed(() => this.preferences().filter(preference => preference.emailFrequency === 'immediate').length);
	readonly digestEnabledCount = computed(() =>
		this.preferences().filter(preference => preference.emailFrequency === 'daily' || preference.emailFrequency === 'weekly').length
	);
	readonly hasChanges = computed(() => {
		const original = this.originalPreferences();
		return this.preferences().some(preference => {
			const baseline = original.find(item => item.type === preference.type);
			return (
				baseline?.inAppEnabled !== preference.inAppEnabled ||
				baseline?.emailEnabled !== preference.emailEnabled ||
				baseline?.emailFrequency !== preference.emailFrequency
			);
		});
	});

	ngOnInit() {
		this.load();
	}

	load() {
		this.loading.set(true);
		this.notificationService
			.getPreferences()
			.pipe(finalize(() => this.loading.set(false)))
			.subscribe({
				next: preferences => {
					this.preferences.set(preferences);
					this.originalPreferences.set(preferences);
				},
				error: () => this.toastService.show('Could not load notification preferences', 'error'),
			});
	}

	toggle(type: string, channel: 'inAppEnabled' | 'emailEnabled', enabled: boolean) {
		this.preferences.update(preferences =>
			preferences.map(preference =>
				preference.type === type ? this.applyChannelToggle(preference, channel, enabled) : preference
			)
		);
	}

	setEmailFrequency(type: string, frequency: NotificationEmailFrequency) {
		this.preferences.update(preferences =>
			preferences.map(preference =>
				preference.type === type
					? { ...preference, emailEnabled: frequency !== 'off', emailFrequency: frequency }
					: preference
			)
		);
	}

	setAll(enabled: boolean) {
		this.preferences.update(preferences =>
			preferences.map(preference => ({
				...preference,
				inAppEnabled: enabled,
				emailEnabled: enabled,
				emailFrequency: enabled ? 'immediate' : 'off',
			}))
		);
	}

	reset() {
		this.preferences.set(this.originalPreferences());
	}

	save() {
		if (!this.hasChanges() || this.saving()) {
			return;
		}

		this.saving.set(true);
		this.notificationService
			.updatePreferences(this.preferences())
			.pipe(finalize(() => this.saving.set(false)))
			.subscribe({
				next: preferences => {
					this.preferences.set(preferences);
					this.originalPreferences.set(preferences);
					this.toastService.show('Notification preferences saved', 'success');
				},
				error: () => this.toastService.show('Could not save notification preferences', 'error'),
			});
	}

	trackByType(_: number, preference: NotificationPreference) {
		return preference.type;
	}

	emailSummary(preference: NotificationPreference) {
		if (!preference.emailEnabled || preference.emailFrequency === 'off') {
			return 'email off';
		}

		if (preference.emailFrequency === 'daily') {
			return 'daily digest';
		}

		if (preference.emailFrequency === 'weekly') {
			return 'weekly digest';
		}

		return 'instant email';
	}

	private applyChannelToggle(
		preference: NotificationPreference,
		channel: 'inAppEnabled' | 'emailEnabled',
		enabled: boolean
	): NotificationPreference {
		if (channel === 'emailEnabled') {
			const emailFrequency = enabled && preference.emailFrequency === 'off' ? 'immediate' : enabled ? preference.emailFrequency : 'off';
			return { ...preference, emailEnabled: enabled, emailFrequency };
		}

		return { ...preference, inAppEnabled: enabled };
	}
}

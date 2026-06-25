import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AccountService } from '../account.service';

@Component({
	selector: 'app-confirm-email',
	imports: [CommonModule, RouterLink],
	templateUrl: './confirm-email.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmEmailComponent implements OnInit {
	status: 'loading' | 'success' | 'error' = 'loading';
	message = 'Confirming your email...';

	private route = inject(ActivatedRoute);
	private accountService = inject(AccountService);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit(): void {
		const userId = this.route.snapshot.queryParamMap.get('userId');
		const token = this.route.snapshot.queryParamMap.get('token');

		if (!userId || !token) {
			this.status = 'error';
			this.message = 'The confirmation link is invalid or incomplete.';
			return;
		}

		this.accountService.confirmEmail({ userId, token }).subscribe({
			next: res => {
				this.status = res.success ? 'success' : 'error';
				this.message = res.success
					? 'Your email has been confirmed. You can sign in now.'
					: 'We could not confirm your email. Request a new confirmation link from account settings.';
				this.cdr.markForCheck();
			},
			error: () => {
				this.status = 'error';
				this.message = 'We could not confirm your email. Request a new confirmation link from account settings.';
				this.cdr.markForCheck();
			},
		});
	}
}

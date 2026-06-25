import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { ROLES } from '../utilities/roles.const';
import { AccountService } from '../guest/account.service';

export const guestGuard: CanActivateFn = () => {
	const accountService = inject(AccountService);
	const router = inject(Router);

	if (!accountService.isLoggedIn()) {
		return true;
	}

	const role = accountService.getRole();
	if (role === ROLES.TRAINER) {
		return router.createUrlTree(['/agency']);
	}
	if (role === ROLES.ADMIN) {
		return router.createUrlTree(['/admin']);
	}
	if (role === ROLES.STUDENT) {
		return router.createUrlTree(['/client']);
	}
	return router.createUrlTree(['/access-denied']);
};

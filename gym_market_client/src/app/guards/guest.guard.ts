import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { ROLES } from '../utilities/roles.const';
import { AccountService } from '../guest/account.service';

export const guestGuard: CanActivateFn = (route, state) => {
	const accountService = inject(AccountService);
	const router = inject(Router);
	const isLoggedIn = accountService.isLoggedIn();
	const role = accountService.getRole();

	if (isLoggedIn === true) {
		if (role === ROLES.TRAINER) {
			router.navigateByUrl('/agency');
		} else if (role === ROLES.STUDENT || role === ROLES.ADMIN) {
			router.navigateByUrl('/client');
		} else {
			router.navigateByUrl('/access-denied');
		}
		return false;
	}

	return true;
};

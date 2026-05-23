import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { ROLES } from '../utilities/roles.const';
import { AccountService } from '../guest/account.service';

export const agencyGuard: CanActivateFn = (route, state) => {
	const accountService = inject(AccountService);
	const router = inject(Router);

	if (!accountService.isLoggedIn()) {
		return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
	}

	const role = accountService.getRole();
	if (role === ROLES.TRAINER || role === ROLES.ADMIN) {
		return true;
	}

	return router.parseUrl(accountService.defaultLandingUrl());
};

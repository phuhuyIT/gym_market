import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AccountService } from '../guest/account.service';
import { ROLES } from '../utilities/roles.const';

export const adminGuard: CanActivateFn = (route, state) => {
	const accountService = inject(AccountService);
	const router = inject(Router);

	if (!accountService.isLoggedIn()) {
		return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
	}

	return accountService.getRole() === ROLES.ADMIN
		? true
		: router.parseUrl(accountService.defaultLandingUrl());
};

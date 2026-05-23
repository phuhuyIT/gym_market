import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AccountService } from '../guest/account.service';

export const authGuard: CanActivateFn = (route, state) => {
	const accountService = inject(AccountService);
	const router = inject(Router);

	if (accountService.isLoggedIn()) {
		return true;
	}

	return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};

import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { ROLES } from '../utilities/roles.const';
import { AccountService } from '../guest/account.service';

export const agencyGuard: CanActivateFn = (route, state) => {
	const accountService = inject(AccountService);
	const router = inject(Router);
	const isLoggedIn = accountService.isLoggedIn();
	const role = accountService.getRole();

    if (isLoggedIn === true && (role === ROLES.TRAINER || role === ROLES.ADMIN)) {
        return true;
    }

	if (isLoggedIn === true) {
		router.navigateByUrl(accountService.defaultLandingUrl());
	} else {
		router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
	}
	return false;
};

import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { ROLES } from '../utilities/roles.const';
import { AccountService } from '../guest/account.service';

export const preventTrainerGuard: CanActivateFn = (route, state) => {
	const accountService = inject(AccountService);
	const router = inject(Router);
	const isLoggedIn = accountService.isLoggedIn();
	const role = accountService.getRole();

	if (isLoggedIn === true && role === ROLES.TRAINER) {
		router.navigateByUrl('/agency');
		return false;
	}

	return true;
};

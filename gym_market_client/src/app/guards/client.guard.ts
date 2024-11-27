import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { ROLES } from '../utilities/roles.const';
import { AccountService } from '../services/account.service';

export const clientGuard: CanActivateFn = (route, state) => {
	const accountService = inject(AccountService);
	const router = inject(Router);
	const isLoggedIn = accountService.isLogedIn();
	const role = accountService.getRole();

	if (isLoggedIn === true && role === ROLES.STUDENT) {
		return true;
	}

	router.navigateByUrl('/');
	return false;
};

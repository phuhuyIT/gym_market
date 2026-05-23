import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { ROLES } from '../utilities/roles.const';
import { AccountService } from '../guest/account.service';

export const preventTrainerGuard: CanActivateFn = () => {
	const accountService = inject(AccountService);
	const router = inject(Router);

	if (accountService.isLoggedIn() && accountService.getRole() === ROLES.TRAINER) {
		return router.createUrlTree(['/agency']);
	}

	return true;
};

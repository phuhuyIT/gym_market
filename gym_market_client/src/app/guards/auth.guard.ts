import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../pages-client/account/account.service';

export const authGuard: CanActivateFn = (route, state) => {
	const accountService = inject(AccountService);
	const router = inject(Router);

	const role = route.data['role'];
	const canAccess: boolean = accountService.hasRole(role);
	return canAccess === true ? canAccess : router.navigateByUrl('/access-denied');
};

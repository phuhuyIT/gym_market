import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { ROLES } from '../utilities/roles.const';
import { AccountService } from '../services/account.service';

export const guestGuard: CanActivateFn = (route, state) => {
	const accountService = inject(AccountService);
	const router = inject(Router);
	const isLoggedIn = accountService.isLogedIn();
	const role = accountService.getRole();

    console.log(isLoggedIn);
    

	if (isLoggedIn == true) {
		if (role === ROLES.TRAINER) {
			router.navigateByUrl('/agency');
		} else if (role === ROLES.STUDENT) {
			router.navigateByUrl('/client');
		}
		else {
            router.navigateByUrl('/access-denied');
        }
		return false;
	}
	// router.navigateByUrl('/');

	return true;
};

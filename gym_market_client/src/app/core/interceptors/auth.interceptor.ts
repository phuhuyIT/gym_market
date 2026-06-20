import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from '../../guest/account.service';
import { catchError, switchMap, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
	const accountService = inject(AccountService);
	const router = inject(Router);

	const token = accountService.token;
	let authReq = req;

	if (token && !req.headers.has('Authorization')) {
		authReq = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
	}

	return next(authReq).pipe(
		catchError((error: HttpErrorResponse) => {
			if (error.status === 401 && accountService.refreshToken && !req.url.includes('refresh-token')) {
				return accountService.apiRefreshToken().pipe(
					switchMap(res => {
						if (res.success) {
							accountService.saveToken(res.token, res.refreshToken);
							const retryReq = req.clone({ setHeaders: { Authorization: `Bearer ${res.token}` } });
							return next(retryReq);
						}
						accountService.logout();
						router.navigateByUrl('/login');
						return throwError(() => error);
					}),
					catchError(() => {
						accountService.logout();
						router.navigateByUrl('/login');
						return throwError(() => error);
					})
				);
			}
			return throwError(() => error);
		})
	);
};

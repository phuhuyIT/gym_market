
import { inject, Injectable } from '@angular/core';
import { UserStore } from '../stores/user.store';
import { patchState } from '@ngrx/signals';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { Login } from './models/login.model';
import { SignUp } from './models/signup.model';
import { StudentSignup } from './models/student-sign-up.model';
import { TrainerSignup } from './models/trainer-sign-up.model';
import { LoginResponse, SignupResponse, UserTokenPayload } from '../core/models/auth.model';
import { ROLES } from '../utilities/roles.const';

@Injectable({
	providedIn: 'root',
})
export class AccountService {
	private _token$ = new BehaviorSubject<string | null>(localStorage.getItem('gym-token'));
	readonly token$ = this._token$.asObservable();

	get token(): string | null {
		return this._token$.value;
	}

	constructor(private http: HttpClient) {
		this.checkLogin();
		this.listenForCrossTabAuthChanges();
	}

	private listenForCrossTabAuthChanges(): void {
		if (typeof window === 'undefined') return;
		window.addEventListener('storage', event => {
			if (event.key !== null && event.key !== 'gym-token') return;
			const fresh = localStorage.getItem('gym-token');
			if (fresh === this._token$.value) return;
			this._token$.next(fresh);
			this.checkLogin();
		});
	}
	userStore = inject(UserStore);

	login(model: Login): Observable<LoginResponse> {
		return this.http.post<LoginResponse>(`${environment.baseApi}/accounts/login`, model);
	}

	signUp(model: SignUp): Observable<SignupResponse> {
		return this.http.post<SignupResponse>(`${environment.baseApi}/accounts/sign-up`, model);
	}

	googleLogin(idToken: string, role?: string): Observable<LoginResponse> {
		return this.http.post<LoginResponse>(`${environment.baseApi}/accounts/google-login`, { idToken, role });
	}

	loadGoogleLibrary(): Promise<void> {
		return new Promise((resolve) => {
			if (window.google?.accounts?.id) {
				resolve();
				return;
			}
			const script = document.createElement('script');
			script.src = 'https://accounts.google.com/gsi/client';
			script.async = true;
			script.defer = true;
			script.onload = () => resolve();
			document.head.appendChild(script);
		});
	}

	studentSignup(model: StudentSignup): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/student`, model);
	}

	trainerSignup(model: TrainerSignup): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/trainer`, model);
	}

	saveToken(token: string): void {
		localStorage.setItem('gym-token', token);
		this._token$.next(token);
		this.checkLogin();
	}

	getRole(): string | null {
		return this.userStore.role();
	}

	isLoggedIn(): boolean {
		const token = this.token;
		if (token === null) {
			return false;
		}
		try {
			const decoded = jwtDecode<UserTokenPayload>(token);
			if (typeof decoded?.exp !== 'number' || Date.now() >= decoded.exp * 1000) {
				this.logout();
				return false;
			}
			return true;
		} catch {
			this.logout();
			return false;
		}
	}

	checkLogin(): void {
		const token = this.token;
		if (token === null) {
			patchState(this.userStore, { fullName: 'Account', id: null, phoneNumber: '', role: null });
		} else {
			try {
				const decoded = jwtDecode<UserTokenPayload>(token);
				patchState(this.userStore, {
					fullName: decoded.unique_name,
					id: decoded.nameid,
					phoneNumber: decoded.homePhone,
					trainerId: decoded.trainerId,
					studentId: decoded.studentId,
					avatar: decoded.avatar,
					role: decoded.role || null,
				});
			} catch (e) {
				this.logout();
			}
		}
	}

	defaultLandingUrl(): string {
		const role = this.getRole();
		if (role === ROLES.TRAINER) return '/agency';
		if (role === ROLES.STUDENT || role === ROLES.ADMIN) return '/client';
		return '/access-denied';
	}

	isSafeReturnUrl(url: string | null | undefined): url is string {
		return !!url && url.startsWith('/') && !url.startsWith('//');
	}

	logout(): void {
		localStorage.removeItem('gym-token');
		this._token$.next(null);
		patchState(this.userStore, { fullName: 'Welcome', id: null, phoneNumber: '', role: null });
	}
}

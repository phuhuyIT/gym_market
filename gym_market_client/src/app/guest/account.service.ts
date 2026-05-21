
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
import {
	LoginResponse,
	SignupResponse,
	UserTokenPayload,
} from '../core/models/auth.model';

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
			if ((window as any).google?.accounts?.id) {
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

	studentSignup(model: StudentSignup): Observable<any> {
		return this.http.post(`${environment.baseApi}/student`, model);
	}

	trainerSignup(model: TrainerSignup): Observable<any> {
		return this.http.post(`${environment.baseApi}/trainer`, model);
	}

	saveToken(token: string): void {
		localStorage.setItem('gym-token', token);
		this._token$.next(token);
		this.checkLogin();
	}

	hasRole(role: string): boolean {
		if (this.token === null) {
			return false;
		}
		const decoded: any = jwtDecode(this.token);
		const roleIn = decoded.role;
		return role === roleIn;
	}

	getRole(): string | null {
		if (this.token === null) {
			return null;
		}

		const decoded: any = jwtDecode(this.token);
		const roleIn = decoded.role;
		return roleIn;
	}

	isLoggedIn(): boolean {
		return this.token !== null;
	}

	checkLogin(): void {
		const token = this.token;
		if (token === null) {
			patchState(this.userStore, { fullName: 'Account', id: null, phoneNumber: '' });
		} else {
			try {
				const decoded: any = jwtDecode(token);
				patchState(this.userStore, {
					fullName: decoded.unique_name,
					id: decoded.nameid,
					phoneNumber: decoded.homePhone,
					trainerId: decoded.trainerId,
					studentId: decoded.studentId,
					avatar: decoded.avatar,
				});
			} catch (e) {
				this.logout();
			}
		}
	}

	logout(): void {
		localStorage.removeItem('gym-token');
		this._token$.next(null);
		patchState(this.userStore, { fullName: 'Welcome', id: null, phoneNumber: '' });
	}
}

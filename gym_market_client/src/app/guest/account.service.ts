
import { inject, Injectable } from '@angular/core';
import { UserStore } from '../stores/user.store';
import { patchState } from '@ngrx/signals';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, tap } from 'rxjs';
import { Login } from './models/login.model';
import { SignUp } from './models/signup.model';
import { StudentSignup } from './models/student-sign-up.model';
import { TrainerSignup } from './models/trainer-sign-up.model';
import {
	ApiResponse,
	AvatarUploadResponse,
	Enable2FAResponse,
	LockoutStatusResponse,
	LoginResponse,
	SignupResponse,
	UserTokenPayload,
} from '../core/models/auth.model';
import { ROLES } from '../utilities/roles.const';
import { STORAGE_KEYS } from '../utilities/storage-keys.const';

const TOKEN_KEY = STORAGE_KEYS.token;
const REFRESH_TOKEN_KEY = STORAGE_KEYS.refreshToken;

@Injectable({
	providedIn: 'root',
})
export class AccountService {
	private _token$ = new BehaviorSubject<string | null>(localStorage.getItem(TOKEN_KEY));
	readonly token$ = this._token$.asObservable();

	get token(): string | null {
		return this._token$.value;
	}

	get refreshToken(): string | null {
		return localStorage.getItem(REFRESH_TOKEN_KEY);
	}

	constructor(private http: HttpClient) {
		this.checkLogin();
		this.listenForCrossTabAuthChanges();
	}

	private listenForCrossTabAuthChanges(): void {
		if (typeof window === 'undefined') return;
		window.addEventListener('storage', event => {
			if (event.key !== null && event.key !== TOKEN_KEY) return;
			const fresh = localStorage.getItem(TOKEN_KEY);
			if (fresh === this._token$.value) return;
			this._token$.next(fresh);
			this.checkLogin();
		});
	}
	userStore = inject(UserStore);

	// ── Auth ──────────────────────────────────────────────────────

	login(model: Login): Observable<LoginResponse> {
		return this.http.post<LoginResponse>(`${environment.baseApi}/accounts/login`, model);
	}

	signUp(model: SignUp): Observable<SignupResponse> {
		return this.http.post<SignupResponse>(`${environment.baseApi}/accounts/sign-up`, model);
	}

	googleLogin(idToken: string, role?: string): Observable<LoginResponse> {
		return this.http.post<LoginResponse>(`${environment.baseApi}/accounts/google-login`, { idToken, role });
	}

	apiRefreshToken(): Observable<LoginResponse> {
		const rt = this.refreshToken;
		if (!rt) return of({ success: false, token: '', refreshToken: '', message: '', errors: ['NO_REFRESH_TOKEN'] });
		return this.http.post<LoginResponse>(`${environment.baseApi}/accounts/refresh-token`, { refreshToken: rt });
	}

	apiLogout(): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${environment.baseApi}/accounts/logout`, {});
	}

	// ── Profile ───────────────────────────────────────────────────

	updateProfile(model: { fullName?: string; address?: string }): Observable<ApiResponse> {
		return this.http.put<ApiResponse>(`${environment.baseApi}/accounts/profile`, model);
	}

	changePassword(model: { currentPassword: string; newPassword: string; confirmNewPassword: string }): Observable<ApiResponse> {
		return this.http.put<ApiResponse>(`${environment.baseApi}/accounts/change-password`, model);
	}

	uploadAvatar(file: File): Observable<AvatarUploadResponse> {
		const formData = new FormData();
		formData.append('file', file);
		return this.http.post<AvatarUploadResponse>(`${environment.baseApi}/accounts/avatar`, formData).pipe(
			tap(res => {
				if (res.success && res.avatarUrl) {
					// Reflect the new avatar in the store immediately, and refresh the
					// JWT so the avatar claim survives a page reload (checkLogin re-reads
					// it from the token). Without this, reloading reverts to the old avatar.
					patchState(this.userStore, { avatar: res.avatarUrl });
					this.refreshSession();
				}
			})
		);
	}

	// Rotates the access token using the stored refresh token. The new JWT is rebuilt
	// from the current DB user, so any server-side profile change (e.g. avatar) is
	// picked up. Fire-and-forget: if there's no refresh token the in-session store
	// update above still stands for the rest of this session.
	private refreshSession(): void {
		this.apiRefreshToken().subscribe({
			next: res => {
				if (res.success && res.token) {
					this.saveToken(res.token, res.refreshToken);
				}
			},
			error: () => {},
		});
	}

	// ── Email Confirmation ────────────────────────────────────────

	sendEmailConfirmation(): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${environment.baseApi}/accounts/send-email-confirmation`, {});
	}

	confirmEmail(model: { userId: string; token: string }): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${environment.baseApi}/accounts/confirm-email`, model);
	}

	// ── Two-Factor Authentication ─────────────────────────────────

	enable2FA(): Observable<Enable2FAResponse> {
		return this.http.post<Enable2FAResponse>(`${environment.baseApi}/accounts/2fa/enable`, {});
	}

	verify2FA(code: string): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${environment.baseApi}/accounts/2fa/verify`, { code });
	}

	disable2FA(): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${environment.baseApi}/accounts/2fa/disable`, {});
	}

	// ── Lockout (Admin) ───────────────────────────────────────────

	getLockoutStatus(userId: string): Observable<LockoutStatusResponse> {
		return this.http.get<LockoutStatusResponse>(`${environment.baseApi}/accounts/lockout/${userId}`);
	}

	unlockAccount(userId: string): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${environment.baseApi}/accounts/lockout/${userId}/unlock`, {});
	}

	// ── Google OAuth ──────────────────────────────────────────────

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

	// ── Token Management ──────────────────────────────────────────

	saveToken(token: string, refreshToken?: string): void {
		localStorage.setItem(TOKEN_KEY, token);
		if (refreshToken) {
			localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
		}
		this._token$.next(token);
		this.checkLogin();
	}

	getRole(): string | null {
		return this.userStore.role();
	}

	// The JWT may carry one role (string) or several (array). Reduce it to the
	// single highest-priority recognized role so route guards can compare directly.
	private normalizeRole(role: string | string[] | null | undefined): string | null {
		const roles = Array.isArray(role) ? role : role ? [role] : [];
		const priority = [ROLES.ADMIN, ROLES.TRAINER, ROLES.STUDENT];
		return priority.find(r => roles.includes(r)) ?? null;
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
					email: decoded.email,
					role: this.normalizeRole(decoded.role),
				});
			} catch (e) {
				this.logout();
			}
		}
	}

	defaultLandingUrl(): string {
		const role = this.getRole();
		if (role === ROLES.TRAINER) return '/agency';
		if (role === ROLES.ADMIN) return '/admin';
		if (role === ROLES.STUDENT) return '/client';
		return '/access-denied';
	}

	isSafeReturnUrl(url: string | null | undefined): url is string {
		return !!url && url.startsWith('/') && !url.startsWith('//');
	}

	logout(): void {
		localStorage.removeItem(TOKEN_KEY);
		localStorage.removeItem(REFRESH_TOKEN_KEY);
		this._token$.next(null);
		patchState(this.userStore, { fullName: 'Welcome', id: null, phoneNumber: '', role: null });
	}
}

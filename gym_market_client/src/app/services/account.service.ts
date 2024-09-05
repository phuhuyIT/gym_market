import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Login } from '../models/account/login.model';
import { SignUp } from '../models/account/signup.model';
import { environment } from '../../environments/environment.development';
import { jwtDecode } from 'jwt-decode';
import { UserStore } from '../stores/user.store';
import { patchState } from '@ngrx/signals';

@Injectable({
	providedIn: 'root',
})
export class AccountService {
	private token: string | null = null;
	constructor(private http: HttpClient) {}
	userStore = inject(UserStore);

	login(model: Login) {
		return this.http.post(`${environment.baseApi}/accounts/login`, model);
	}

	logout() {
		localStorage.removeItem('gym-token');
		patchState(this.userStore, { fullName: 'Welcome', id: null, phoneNumber: '' });
	}

	signUp(model: SignUp) {
		return this.http.post(`${environment.baseApi}/accounts/sign-up`, model);
	}

	saveToken(token: string) {
		this.token = token;
		localStorage.setItem('gym-token', token);
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

	isLogedIn() {
		return this.token !== null;
	}

	checkLogin() {
		const token = localStorage.getItem('gym-token');
		if (token === null) {
			patchState(this.userStore, { fullName: 'Account', id: null, phoneNumber: '' });
			this.token = null;
		} else {
			const decoded: any = jwtDecode(token);
			this.token = token;
			patchState(this.userStore, {
				fullName: decoded.unique_name,
				id: decoded.nameid,
				phoneNumber: decoded.homePhone,
			});
		}
	}
}

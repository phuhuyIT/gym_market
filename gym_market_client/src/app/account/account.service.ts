
import { inject, Injectable } from '@angular/core';
import { UserStore } from '../stores/user.store';
import { patchState } from '@ngrx/signals';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Login } from './models/login.model';
import { SignUp } from './models/signup.model';
import { StudentSignup } from './models/student-sign-up.model';
import { TrainerSignup } from './models/trainer-sign-up.model';

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

	signUp(model: SignUp) {
		return this.http.post(`${environment.baseApi}/accounts/sign-up`, model);
	}

	studentSignup(model: StudentSignup) {
		return this.http.post(`${environment.baseApi}/student`, model);
	}

	trainerSignup(model: TrainerSignup) {
		return this.http.post(`${environment.baseApi}/trainer`, model);
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

	getRole() {
		if (this.token === null) {
			return null;
		}

		const decoded: any = jwtDecode(this.token);
		const roleIn = decoded.role;
		return roleIn;
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
                trainerId: decoded.trainerId,
                studentId: decoded.studentId,
			});
		}
	}


    logout() {
		localStorage.removeItem('gym-token');
        this.token = null;
		patchState(this.userStore, { fullName: 'Welcome', id: null, phoneNumber: '' });
	}
}

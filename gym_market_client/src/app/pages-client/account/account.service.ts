import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { jwtDecode } from 'jwt-decode';
import { UserStore } from '../../stores/user.store';
import { patchState } from '@ngrx/signals';

@Injectable({
	providedIn: 'root',
})
export class AccountService {
	// private token: string | null = null;
	// constructor(private http: HttpClient) {}
	userStore = inject(UserStore);


    logout() {
		localStorage.removeItem('gym-token');
		patchState(this.userStore, { fullName: 'Welcome', id: null, phoneNumber: '' });
	}
}

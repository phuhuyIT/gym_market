import { inject, Injectable } from '@angular/core';
import { UserStore } from '../stores/user.store';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Login } from './models/login.model';
import { patchState } from '@ngrx/signals';
import { SignUp } from './models/signup.model';
import { jwtDecode } from 'jwt-decode';
import { TrainerSignup } from './models/trainer-sign-up.model';
import { StudentSignup } from './models/student-sign-up.model';

@Injectable({
	providedIn: 'root',
})
export class GuestService {
	
}

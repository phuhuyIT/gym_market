import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { UpdateUserDto } from './models/update-user.dto';
import { UserInfoResponse } from '../core/models/auth.model';

@Injectable({
	providedIn: 'root',
})
export class UserService {
	constructor(private http: HttpClient) {}

	// Returns the authenticated caller's own profile; the backend resolves the
	// user from the JWT, so no id is sent.
	getUserInfo(): Observable<UserInfoResponse> {
		return this.http.get<UserInfoResponse>(
			`${environment.baseApi}/users/get-user-info`
		);
	}

	updateUser(model: UpdateUserDto): Observable<void> {
		return this.http.put<void>(`${environment.baseApi}/users/update-user`, model);
	}
}

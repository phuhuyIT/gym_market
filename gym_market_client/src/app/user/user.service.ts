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

	getUserInfo(userId: string | null): Observable<UserInfoResponse> {
		return this.http.get<UserInfoResponse>(
			`${environment.baseApi}/users/get-user-info/${userId}`
		);
	}

	updateUser(model: UpdateUserDto): Observable<any> {
		return this.http.put(`${environment.baseApi}/users/update-user`, model);
	}
}

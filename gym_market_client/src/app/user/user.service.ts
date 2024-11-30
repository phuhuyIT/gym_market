import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { UpdateUserDto } from './models/update-user.dto';

@Injectable({
	providedIn: 'root',
})
export class UserService {
	constructor(private http: HttpClient) {}

	getUserInfo(userId: string | null) {
		return this.http.get(`${environment.baseApi}/users/get-user-info/${userId}`);
	}

	updateUser(model: UpdateUserDto) {
		return this.http.put(`${environment.baseApi}/users/update-user`, model);
	}
}

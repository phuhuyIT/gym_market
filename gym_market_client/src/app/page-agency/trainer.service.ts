import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Trainer, UpdateTrainerProfileDto } from '../core/models/trainer.model';

@Injectable({
	providedIn: 'root',
})
export class TrainerService {
	constructor(private http: HttpClient) {}

	getTrainerInfo(trainerId: string): Observable<Trainer> {
		return this.http.get<Trainer>(`${environment.baseApi}/trainer/${trainerId}`);
	}

	updateTrainerProfile(model: UpdateTrainerProfileDto, trainerId: string): Observable<void> {
		return this.http.put<void>(`${environment.baseApi}/trainer/${trainerId}`, model);
	}

	getTrainers(): Observable<Trainer[]> {
		return this.http.get<Trainer[]>(`${environment.baseApi}/trainer`);
	}
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { PagedResult } from '../core/models/paged-result.model';
import { Trainer, TrainerSearch, UpdateTrainerProfileDto } from '../core/models/trainer.model';

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

	searchTrainersPaged(
		search = '',
		pageIndex = 1,
		pageSize = 12,
		category = '',
		eliteOnly = false
	): Observable<PagedResult<TrainerSearch>> {
		return this.http.get<PagedResult<TrainerSearch>>(`${environment.baseApi}/trainer/search`, {
			params: {
				search: search.trim(),
				pageIndex: pageIndex.toString(),
				pageSize: pageSize.toString(),
				category: category.trim(),
				eliteOnly: eliteOnly.toString(),
			},
		});
	}
}

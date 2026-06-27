import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
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
		return this.searchTrainersPaged('', 1, 50).pipe(
			map(result =>
				result.items.map(item => ({
					trainerId: item.trainerId,
					userId: item.userId ?? '',
					name: item.name ?? item.fullName ?? '',
					email: item.email ?? '',
					profilePicture: item.profilePicture ?? '',
					bio: item.bio ?? '',
					certification: item.certification ?? '',
					category: item.category ?? '',
					experience: item.experience ?? 0,
					rating: item.rating ?? 0,
					createdAt: item.createdAt ?? '',
					updatedAt: '',
				}))
			)
		);
	}

	searchTrainersPaged(
		search = '',
		pageIndex = 1,
			pageSize = 12,
			category = '',
			eliteOnly = false,
			minRating?: number,
			maxRating?: number,
			minExperience?: number,
			maxExperience?: number,
			status = ''
		): Observable<PagedResult<TrainerSearch>> {
			const params: Record<string, string> = {
				search: search.trim(),
				pageIndex: pageIndex.toString(),
				pageSize: pageSize.toString(),
				category: category.trim(),
				eliteOnly: eliteOnly.toString(),
			};

			if (minRating !== undefined) params['minRating'] = minRating.toString();
			if (maxRating !== undefined) params['maxRating'] = maxRating.toString();
			if (minExperience !== undefined) params['minExperience'] = minExperience.toString();
			if (maxExperience !== undefined) params['maxExperience'] = maxExperience.toString();
			if (status) params['status'] = status;

			return this.http.get<PagedResult<TrainerSearch>>(`${environment.baseApi}/trainer/search`, { params });
		}
}

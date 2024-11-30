import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { UpdateTrainerProfileDto } from './models/update-trainer.dto';

@Injectable({
	providedIn: 'root',
})
export class TrainerService {
	constructor(private http: HttpClient) {}

	getTrainerInfo(trainerId: string) {
		return this.http.get(`${environment.baseApi}/trainer/${trainerId}`);
	}

    updateTrainerProfile(model: UpdateTrainerProfileDto, trainerId: string) {
        return this.http.put(`${environment.baseApi}/trainer/${trainerId}`, model);
    }
}

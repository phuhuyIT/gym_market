import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
	BodyFatMeasurements,
	BodyFatPredictionResponse,
} from '../core/models/body-fat.model';
import { environment } from '../../environments/environment.development';

@Injectable({
	providedIn: 'root',
})
export class BodyFatService {
	constructor(private http: HttpClient) {}

	predictBodyFat(model: BodyFatMeasurements): Observable<BodyFatPredictionResponse> {
		return this.http.post<BodyFatPredictionResponse>(
			`${environment.pythonApi}/predict_bodyfat`,
			model
		);
	}

	predictByImageMale(form: FormData): Observable<BodyFatPredictionResponse> {
		return this.http.post<BodyFatPredictionResponse>(
			`${environment.pythonApi}/predict_image_male/`,
			form
		);
	}

	predictByImageFemale(form: FormData): Observable<BodyFatPredictionResponse> {
		return this.http.post<BodyFatPredictionResponse>(
			`${environment.pythonApi}/predict_image_female/`,
			form
		);
	}
}

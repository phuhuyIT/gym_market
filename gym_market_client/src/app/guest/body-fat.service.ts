import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
	BodyFatMeasurements,
	BodyFatPredictionResponse,
} from '../core/models/body-fat.model';

@Injectable({
	providedIn: 'root',
})
export class BodyFatService {
	constructor(private http: HttpClient) {}

	predictBodyFat(model: BodyFatMeasurements): Observable<BodyFatPredictionResponse> {
		return this.http.post<BodyFatPredictionResponse>(
			'http://127.0.0.1:8000/predict_bodyfat',
			model
		);
	}

	predictByImageMale(form: FormData): Observable<BodyFatPredictionResponse> {
		return this.http.post<BodyFatPredictionResponse>(
			'http://127.0.0.1:8000/predict_image_male/',
			form
		);
	}

	predictByImageFemale(form: FormData): Observable<BodyFatPredictionResponse> {
		return this.http.post<BodyFatPredictionResponse>(
			'http://127.0.0.1:8000/predict_image_female/',
			form
		);
	}
}

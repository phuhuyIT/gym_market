import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
	providedIn: 'root',
})
export class BodyFatService {
	constructor(private http: HttpClient) {}

	predictBodyFat(model: any) {
		return this.http.post('http://127.0.0.1:8000/predict_bodyfat', model);
	}

	predictByImageMale(form: any) {
		return this.http.post('http://127.0.0.1:8000/predict_image_male/', form);
	}

    predictByImageFemale(form: any) {
		return this.http.post('http://127.0.0.1:8000/predict_image_female/', form);
	}
}

import { Component, Renderer2, inject, DestroyRef, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BodyFatService } from '../body-fat.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BodyFatPredictionResponse } from '../../core/models/body-fat.model';

@Component({
	selector: 'app-predict-body-fat-by-image',
	standalone: true,
	imports: [FormsModule],
	templateUrl: './predict-body-fat-by-image.component.html',
	styleUrl: './predict-body-fat-by-image.component.scss',
})
export class PredictBodyFatByImageComponent implements OnInit {
	gender: string = 'Male';
	imageToPredict: File | null = null;
	imageToShow: string | ArrayBuffer | null = null;

	predictResult: number = 0;
	showPredictResult: boolean = false;

	private destroyRef = inject(DestroyRef);

	constructor(private bodyFatService: BodyFatService, private renderer: Renderer2) {}

	ngOnInit() {
		this.imageToPredict = null;
		this.imageToShow = null;
	}

	onSelectImage(event: Event) {
		const target = event.target as HTMLInputElement;
		const file = target.files?.[0];
		if (!file) return;

		this.imageToPredict = file;

		const reader = new FileReader();
		reader.readAsDataURL(file);
		reader.onload = e => {
			const data = e.target?.result;
			this.imageToShow = data ?? null;
		};

		this.predict();
	}

	predict() {
		if (!this.imageToPredict) return;

		const form = new FormData();
		form.append('file', this.imageToPredict);

		if (this.gender === 'Male') {
			this.bodyFatService.predictByImageMale(form).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
				next: (res: BodyFatPredictionResponse) => {
					this.predictResult = res['Predicted Label'] ?? 0;
					this.showPredictResult = true;
				},
				error: _err => {
				},
			});
		} else {
			this.bodyFatService.predictByImageFemale(form).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
				next: (res: BodyFatPredictionResponse) => {
					this.predictResult = res['Predicted Label'] ?? 0;
					this.showPredictResult = true;
				},
				error: _err => {
				},
			});
		}
	}

	onCloseResult() {
		this.renderer.removeClass(document.body, 'overflow-hidden');
		this.showPredictResult = false;
		this.predictResult = 0;
	}

	clearImage() {
		this.imageToPredict = null;
		this.imageToShow = null;
	}
}

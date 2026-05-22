import { Component, Renderer2, inject, DestroyRef, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BodyFatService } from '../body-fat.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BodyFatPredictionResponse } from '../../core/models/body-fat.model';
import { CommonModule } from '@angular/common';
import { GmInputComponent, GmButtonComponent, GmCardComponent } from '../../shared';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-predict-body-fat-by-image',
    imports: [CommonModule, FormsModule, GmInputComponent, GmButtonComponent, GmCardComponent, RouterLink],
    templateUrl: './predict-body-fat-by-image.component.html',
    styleUrl: './predict-body-fat-by-image.component.scss'
})
export class PredictBodyFatByImageComponent implements OnInit {
	gender: string = 'Male';
	imageToPredict: File | null = null;
	imageToShow: string | ArrayBuffer | null = null;

	predictResult: number = 0;
	showPredictResult: boolean = false;
	loading = false;

	private destroyRef = inject(DestroyRef);
	private bodyFatService = inject(BodyFatService);
	private renderer = inject(Renderer2);

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
	}

	predict() {
		if (!this.imageToPredict) return;

		this.loading = true;
		const form = new FormData();
		form.append('file', this.imageToPredict);

		const obs = this.gender === 'Male' 
			? this.bodyFatService.predictByImageMale(form)
			: this.bodyFatService.predictByImageFemale(form);

		obs.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (res: BodyFatPredictionResponse) => {
				this.predictResult = res['Predicted Label'] ?? 0;
				this.showPredictResult = true;
				this.loading = false;
				this.renderer.addClass(document.body, 'no-scroll');
			},
			error: () => {
				this.loading = false;
			},
		});
	}

	onCloseResult() {
		this.renderer.removeClass(document.body, 'no-scroll');
		this.showPredictResult = false;
	}

	clearImage() {
		this.imageToPredict = null;
		this.imageToShow = null;
	}
}

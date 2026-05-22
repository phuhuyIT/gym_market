import { Component, inject, Renderer2, DestroyRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BodyFatService } from '../body-fat.service';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BodyFatMeasurements, BodyFatPredictionResponse } from '../../core/models/body-fat.model';
import { CommonModule } from '@angular/common';
import { GmInputComponent, GmButtonComponent, GmCardComponent } from '../../shared';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-check-bmi',
    imports: [CommonModule, FormsModule, GmInputComponent, GmButtonComponent, GmCardComponent, RouterLink],
    templateUrl: './check-bmi.component.html',
    styleUrl: './check-bmi.component.scss'
})
export class CheckBmiComponent {
	bmi = 0;
	age = 25;
	weight = 70;
	height = 175;
	neckCircumference = 36;
	chestCircumference = 95;
	abdomen_2_Circumference = 85;
	thighCircumference = 55;
	kneeCircumference = 38;
	biceps_extended_Circumference = 32;

	showBodyFat = false;
	bodyFat = 0;
	loading = false;

	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);
	private bodyFatService = inject(BodyFatService);
	private renderer = inject(Renderer2);

	calculateBMI() {
		const hMeter = this.height / 100;
		this.bmi = this.weight / (hMeter * hMeter);
	}

	calBodyFat() {
		const model: BodyFatMeasurements = {
			Age: this.age,
			Weight: this.weight / 0.453592,
			Height: this.height / 2.54,
			Neck: this.neckCircumference,
			Chest: this.chestCircumference,
			Abdomen: this.abdomen_2_Circumference,
			Thigh: this.thighCircumference,
			Knee: this.kneeCircumference,
			Biceps: this.biceps_extended_Circumference,
		};

		this.loading = true;
		patchState(this.loader, { isShow: true });
		
		this.bodyFatService.predictBodyFat(model).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (res: BodyFatPredictionResponse) => {
				this.bodyFat = Number(res['Predicted Body Fat Percentage']?.toFixed(2) ?? 0);
				this.showBodyFat = true;
				this.loading = false;
				patchState(this.loader, { isShow: false });
				this.calculateBMI();
				this.renderer.addClass(document.body, 'no-scroll');
			},
			error: () => {
				this.loading = false;
				patchState(this.loader, { isShow: false });
			},
		});
	}

	onCloseResult() {
		this.renderer.removeClass(document.body, 'no-scroll');
		this.showBodyFat = false;
	}
}

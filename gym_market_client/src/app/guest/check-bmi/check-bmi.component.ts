import { Component, inject, Renderer2 } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BodyFatService } from '../body-fat.service';
import { NoticeModalStore } from '../../stores/notice.store';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';

@Component({
	selector: 'app-check-bmi',
	standalone: true,
	imports: [FormsModule],
	templateUrl: './check-bmi.component.html',
	styleUrl: './check-bmi.component.scss',
})
export class CheckBmiComponent {

	bmi: number = 0;

	// body fast
	age: number = 10;
	weight: number = 0;
	height: number = 1;
	neckCircumference: number = 0;
	chestCircumference: number = 0; // (cm)
	abdomen_2_Circumference: number = 0; //(cm)
	thighCircumference: number = 0; //(cm)
	kneeCircumference: number = 0; //(cm)
	biceps_extended_Circumference: number = 0; //(cm)

	notice = inject(NoticeModalStore);
	error = inject(ErrorModalStore);

	showBodyFat: boolean = false;
	bodyFat: number = 0;

	constructor(private bodyFatService: BodyFatService, private renderer: Renderer2) {}

	calculateBMI() {
		this.bmi = (this.weight * 703) / (this.height * this.height);
	}

	calBodyFat() {
		if (this.weight < 20) {
			patchState(this.error, {
				isShow: true,
				errors: ['Weight must be greater than or equal 20kg'],
			});
			return;
		}

		if (this.weight > 200) {
			patchState(this.error, { isShow: true, errors: ['Weight must be less than 200kg'] });
			return;
		}

		if (this.height < 100) {
			patchState(this.error, {
				isShow: true,
				errors: ['Height must be greater than or equal 100cm'],
			});
			return;
		}

		if (this.height > 250) {
			patchState(this.error, { isShow: true, errors: ['Height must be less than 250cm'] });
			return;
		}

		const model = {
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

		this.bodyFatService.predictBodyFat(model).subscribe({
			next: (res: any) => {
				console.log(res);
				this.bodyFat = res['Predicted Body Fat Percentage'].toFixed(2);
				this.showBodyFat = true;
				this.renderer.addClass(document.body, 'overflow-hidden');
			},
			error: err => {
				console.log(err);
			},
		});
	}

	onCloseResult() {
		this.renderer.removeClass(document.body, 'overflow-hidden');
		this.showBodyFat = false;
	}
}

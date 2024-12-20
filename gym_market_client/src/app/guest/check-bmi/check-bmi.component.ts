import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
	selector: 'app-check-bmi',
	standalone: true,
	imports: [FormsModule, ],
	templateUrl: './check-bmi.component.html',
	styleUrl: './check-bmi.component.scss',
})
export class CheckBmiComponent {
    gender: string = 'male';

	bmi: number = 0;

	// body fast
	age: number = 10;
	weight: number = 0;
	height: number = 1;
	neckCircumference: number = 0;
	chestCircumference: number = 0; // (cm)
	abdomen_2_Circumference: number = 0; //(cm)
	hipCircumference: number = 0; //(cm)
	thighCircumference: number = 0; //(cm)
	kneeCircumference: number = 0; //(cm)
	ankleCircumference: number = 0; //(cm)
	biceps_extended_Circumference: number = 0; //(cm)
	forearmCircumference: number = 0; //(cm)
	wristCircumference: number = 0; //(cm)
    waist: number = 0; //(cm)

	calculateBMI() {
		this.bmi = (this.weight * 703 / (this.height * this.height));
	}
}

import { Component, Renderer2 } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BodyFatService } from '../body-fat.service';

@Component({
	selector: 'app-predicy-body-fast-by-image',
	standalone: true,
	imports: [FormsModule],
	templateUrl: './predicy-body-fast-by-image.component.html',
	styleUrl: './predicy-body-fast-by-image.component.scss',
})
export class PredicyBodyFastByImageComponent {
	gender: string = 'Male';
	imageToPredict: any;
	imageToShow: any;

	predictResult: number = 0;
	showPredictResult: boolean = false;

	constructor(private bodyFatService: BodyFatService, private renderer: Renderer2) {}

	ngOnInit() {
		this.imageToPredict = null;
		this.imageToShow = null;
	}

	onSelectImage(event: any) {
		const file = event.target.files[0];

		this.imageToPredict = file;

		const reader = new FileReader();
		reader.readAsDataURL(file);
		reader.onload = event => {
			const data = event.target?.result;
			this.imageToShow = data;
		};

		this.predict();
	}

	predict() {
		const form = new FormData();
		form.append('file', this.imageToPredict);

		if (this.gender === 'Male') {
			this.bodyFatService.predictByImageMale(form).subscribe({
				next: (res: any) => {
					console.log(res['Predicted Label']);
					this.predictResult = res['Predicted Label'];
                    this.showPredictResult = true;
				},
				error: err => {
					console.log(err);
				},
			});
		} else {
			this.bodyFatService.predictByImageFemale(form).subscribe({
				next: (res: any) => {
					this.predictResult = res['Predicted Label'];
					console.log(res['Predicted Label']);
                    this.showPredictResult = true;
				},
				error: err => {
					console.log(err);
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

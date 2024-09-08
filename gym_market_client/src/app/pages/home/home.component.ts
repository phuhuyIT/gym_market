import { Component, ElementRef, NgModule, ViewChild } from '@angular/core';
import { HeaderComponent } from '../components/header/header.component';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
	selector: 'app-home',
	standalone: true,
	imports: [HeaderComponent, CommonModule, RouterLink],
	templateUrl: './home.component.html',
	styleUrl: './home.component.scss',
})
export class HomeComponent {
	// @ViewChildren('dots') dots!: QueryList<ElementRef>;
	@ViewChild('slides') slides!: ElementRef;
	slideTimer: any;
	slideIndex: number = 1;
	slideWidth: number = 0;
	slideLenth = 4;
	year = 0;

	ngOnInit() {
		this.year = new Date().getFullYear();
	}

	ngAfterViewInit() {
		this.slideWidth = this.slides.nativeElement.offsetWidth;

		this.slideShow();
	}

	private slideShow() {
		this.slideTimer = setInterval(() => {
			if (this.slideIndex >= this.slideLenth) {
				this.slideIndex = 0;
			}
			this.slides.nativeElement.scrollLeft = this.slideIndex * this.slideWidth;

			this.slideIndex++;
		}, 3000);
	}

	onClickSlide(index: number) {
		this.slides.nativeElement.scrollLeft = index * this.slideWidth;
		this.slideIndex = index + 1;
		clearInterval(this.slideTimer);

		this.slideShow();
	}

	ngOnDestroy() {
		clearInterval(this.slideTimer);
	}
}

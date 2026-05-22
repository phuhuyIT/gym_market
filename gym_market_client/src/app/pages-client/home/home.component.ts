
import { Component, ElementRef, ViewChild } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-home',
    imports: [RouterLink],
    templateUrl: './home.component.html',
    styleUrl: './home.component.scss'
})
export class HomeComponent {
	// @ViewChildren('dots') dots!: QueryList<ElementRef>;
	@ViewChild('slides') slides!: ElementRef;
	slideTimer: ReturnType<typeof setInterval> | null = null;
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
		if (this.slideTimer) {
			clearInterval(this.slideTimer);
		}

		this.slideShow();
	}

	ngOnDestroy() {
		if (this.slideTimer) {
			clearInterval(this.slideTimer);
		}
	}
}

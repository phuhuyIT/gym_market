import { Component, ElementRef, ViewChild } from '@angular/core';
import { HeaderComponent } from '../components/header/header.component';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FooterComponent } from '../components/footer/footer.component';

@Component({
	selector: 'app-home',
	standalone: true,
	imports: [CommonModule, RouterLink],
	templateUrl: './home.component.html',
	styleUrl: './home.component.scss',
})
export class HomeComponent {
	// @ViewChildren('dots') dots!: QueryList<ElementRef>;
	@ViewChild('slides') slides!: ElementRef;
	year = 0;

	ngOnInit() {
		this.year = new Date().getFullYear();
	}

	ngAfterViewInit() {
	}

	

	
}

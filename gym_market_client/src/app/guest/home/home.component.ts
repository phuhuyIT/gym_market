import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RevealDirective } from '../../shared/directives/reveal.directive';

@Component({
	selector: 'app-home',
	standalone: true,
	imports: [CommonModule, RouterLink, RevealDirective],
	templateUrl: './home.component.html',
	styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
	year = 0;

	categories = [
		{ name: 'Yoga', icon: '🧘' },
		{ name: 'Cardio', icon: '🏃' },
		{ name: 'Strength', icon: '💪' },
		{ name: 'HIIT', icon: '⚡' },
		{ name: 'Pilates', icon: '🤸' },
		{ name: 'Boxing', icon: '🥊' },
		{ name: 'Dance', icon: '💃' },
		{ name: 'Crossfit', icon: '🏋️' }
	];

	ngOnInit() {
		this.year = new Date().getFullYear();
	}
}

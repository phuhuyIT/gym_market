import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
    showAccountOption = false;

    constructor(private router: Router) {
        
    }

	onShowAccountOption() {
		this.showAccountOption = true;
	}

	onHiddenAccountOption() {
		this.showAccountOption = false;
	}

    onClick() {
        this.router.navigateByUrl('/');
    }
}

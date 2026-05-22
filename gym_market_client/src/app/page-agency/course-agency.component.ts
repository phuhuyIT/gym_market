import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';

import { AccountService } from '../guest/account.service';

@Component({
    selector: 'app-course-agency',
    imports: [RouterOutlet, RouterLink, RouterLinkActive],
    templateUrl: './course-agency.component.html',
    styleUrl: './course-agency.component.scss'
})
export class CourseAgencyComponent {
  private accountService = inject(AccountService);
  private router = inject(Router);

  navLinks = [
    { label: 'Dashboard', path: '/agency/course-list', icon: '📊' },
    { label: 'My Profile', path: '/agency/your-profile', icon: '👤' },
    { label: 'Messages', path: '/chat', icon: '💬' },
    { label: 'Payments', path: '/agency/get-payments', icon: '💰' },
  ];

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('');
  }
}

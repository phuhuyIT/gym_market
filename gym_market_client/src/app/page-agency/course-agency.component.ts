import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-course-agency',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './course-agency.component.html',
  styleUrl: './course-agency.component.scss'
})
export class CourseAgencyComponent {
  navLinks = [
    { label: 'Dashboard', path: '/agency/course-list', icon: '📊' },
    { label: 'My Profile', path: '/agency/your-profile', icon: '👤' },
    { label: 'Messages', path: '/chat', icon: '💬' },
    { label: 'Payments', path: '/agency/get-payments', icon: '💰' },
  ];
}

import { Component, inject } from '@angular/core';
import { HeaderComponent } from "../../components/header/header.component";
import { UserStore } from '../../../stores/user.store';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {
  userStore = inject(UserStore)
}

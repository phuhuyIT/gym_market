import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent {

}

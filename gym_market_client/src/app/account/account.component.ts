import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from './components/footer/footer.component';
import { HeaderComponent } from './components/header/header.component';

@Component({
  selector: 'app-guest',
  standalone: true,
  imports: [RouterOutlet, FooterComponent, HeaderComponent],
  templateUrl: './account.component.html',
  styleUrl: './account.component.scss'
})
export class GuestComponent {

}

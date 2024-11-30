import { Component } from '@angular/core';
import { HeaderComponent } from './components/header/header.component';
import { FooterComponent } from './components/footer/footer.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-pages-client',
  standalone: true,
  imports: [HeaderComponent, FooterComponent, RouterOutlet],
  templateUrl: './pages-client.component.html',
  styleUrl: './pages-client.component.scss'
})
export class PagesClientComponent {

}

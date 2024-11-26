import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ErrorModalComponent } from './components/error-modal/error-modal.component';
import { LoaderComponent } from "./components/loader/loader.component";
import { NoticeComponent } from "./components/notice/notice.component";
import { GuestService } from './guest/guest.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ErrorModalComponent, LoaderComponent, NoticeComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Fitness_Client';
  guestService = inject(GuestService)
  
  ngOnInit() {
    this.guestService.checkLogin();
  }
}

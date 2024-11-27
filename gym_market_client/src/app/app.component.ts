import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ErrorModalComponent } from './components/error-modal/error-modal.component';
import { LoaderComponent } from "./components/loader/loader.component";
import { NoticeComponent } from "./components/notice/notice.component";
import { AccountService } from './services/account.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ErrorModalComponent, LoaderComponent, NoticeComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Fitness_Client';
  accountService = inject(AccountService)
  
  ngOnInit() {
    this.accountService.checkLogin();
  }
}

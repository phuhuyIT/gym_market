import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AccountService } from './pages-client/account/account.service';
import { ErrorModalComponent } from './pages-client/components/error-modal/error-modal.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ErrorModalComponent],
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

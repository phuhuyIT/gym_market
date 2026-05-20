import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ErrorModalComponent } from './components/error-modal/error-modal.component';
import { LoaderComponent } from "./components/loader/loader.component";
import { NoticeComponent } from "./components/notice/notice.component";
import { GmToastComponent } from './shared/components/gm-toast/gm-toast.component';
import { AccountService } from './guest/account.service';
import { ThemeService } from './core/services/theme.service';

import { AmbientComponent } from './shared/components/ambient/ambient.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ErrorModalComponent, LoaderComponent, NoticeComponent, GmToastComponent, AmbientComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Fitness_Client';
  accountService = inject(AccountService);
  themeService = inject(ThemeService);
  
  ngOnInit() {
    this.themeService.init();
    this.accountService.checkLogin();
  }
}

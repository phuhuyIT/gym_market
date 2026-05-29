import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AccountService } from '../../guest/account.service';

@Component({
    selector: 'app-access-denied',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink],
    templateUrl: './access-denied.component.html',
    styleUrl: './access-denied.component.scss'
})
export class AccessDeniedComponent {
    private accountService = inject(AccountService);
    private router = inject(Router);

    logout() {
        this.accountService.logout();
        this.router.navigateByUrl('/login');
    }
}

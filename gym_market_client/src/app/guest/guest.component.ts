import { Component , ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from '../shared/components/footer/footer.component';
import { HeaderComponent } from '../shared/components/header/header.component';

@Component({
    selector: 'app-guest',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterOutlet, FooterComponent, HeaderComponent],
    templateUrl: './guest.component.html',
    styleUrl: './guest.component.scss'
})
export class GuestComponent {

}

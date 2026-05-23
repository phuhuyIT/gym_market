import { Component , ChangeDetectionStrategy } from '@angular/core';
import { HeaderComponent } from '../shared/components/header/header.component';
import { FooterComponent } from '../shared/components/footer/footer.component';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-pages-client',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [HeaderComponent, FooterComponent, RouterOutlet],
    templateUrl: './pages-client.component.html',
    styleUrl: './pages-client.component.scss'
})
export class PagesClientComponent {

}

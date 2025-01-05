import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../guest/components/header/header.component';

@Component({
	selector: 'app-chat',
	standalone: true,
	imports: [RouterOutlet, HeaderComponent],
	templateUrl: './chat.component.html',
	styleUrl: './chat.component.scss',
})
export class ChatComponent {
	
}

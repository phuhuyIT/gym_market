import { Routes } from '@angular/router';
import { ChatComponent } from './chat.component';
import { ChatListComponent } from './chat-list/chat-list.component';
export const routes: Routes = [
	{
		path: '',
        component: ChatComponent,
		children: [
			{ path: '', redirectTo: 'chat-list', pathMatch: 'full' },
			{ path: 'chat-list', component: ChatListComponent, title: 'Chat list' },
		],
	},
];
    
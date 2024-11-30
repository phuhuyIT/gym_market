import { Component, inject } from '@angular/core';
import { UserInfoDto } from '../../user/models/user-info.dto';
import { Router } from '@angular/router';
import { UserService } from '../../user/user.service';
import { UserStore } from '../../stores/user.store';

@Component({
  selector: 'app-your-profile',
  standalone: true,
  imports: [],
  templateUrl: './your-profile.component.html',
  styleUrl: './your-profile.component.scss'
})
export class YourProfileComponent {
    userInfo!: UserInfoDto;
	userStore = inject(UserStore);
	

	constructor(
		private router: Router,
		private userService: UserService
	) {}

	ngOnInit() {
		
	}

	private getUserInfo() {
		
	}

	private getTrainerInfo() {
		
	}
}

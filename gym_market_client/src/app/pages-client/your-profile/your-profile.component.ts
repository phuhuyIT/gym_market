import { Component, inject } from '@angular/core';
import { UserInfoDto } from '../../user/models/user-info.dto';
import { Router, RouterLink } from '@angular/router';
import { UserService } from '../../user/user.service';
import { UserStore } from '../../stores/user.store';
import { StudentInfoDto } from '../models/student-info.dto';
import { StudentService } from '../student.service';

@Component({
	selector: 'app-your-profile',
	standalone: true,
	imports: [RouterLink],
	templateUrl: './your-profile.component.html',
	styleUrl: './your-profile.component.scss',
})
export class YourProfileComponent {
	userInfo!: UserInfoDto;
	userStore = inject(UserStore);
	studentInfo!: StudentInfoDto;

	constructor(
		private studentService: StudentService,
		private router: Router,
		private userService: UserService
	) {}

	ngOnInit() {
		this.userInfo = {
			address: 'https://cdn-icons-png.flaticon.com/512/236/236832.png',
			avatar: '',
			email: '',
			fullName: '',
			id: '',
			phoneNumber: '',
			status: '',
		};

		this.studentInfo = {
			createdAt: new Date(),
			email: '',
			name: '',
			password: '',
			profilePicture: '',
			studentId: '',
			updatedAt: new Date(),
			userId: '',
			healthStatus: '',
		};

		this.getUserInfo();
		this.getTrainerInfo();
	}

	private getUserInfo() {
		if (this.userStore.id() !== null) {
			this.userService.getUserInfo(this.userStore.id()).subscribe({
				next: (res: any) => {
					this.userInfo = { ...res.userInfo };
					if (this.userInfo.avatar === null) {
						this.userInfo.avatar =
							'https://cdn-icons-png.flaticon.com/512/236/236832.png';
					}
				},
				error: error => {
					this.router.navigateByUrl('/login');
				},
			});
		}
	}

	private getTrainerInfo() {
		this.studentService.getStudentInfo(this.userStore.studentId()).subscribe({
			next: (res: any) => {
				this.studentInfo = { ...res };
				if (this.studentInfo.profilePicture === null) {
					this.studentInfo.profilePicture =
						'https://cdn-icons-png.flaticon.com/512/236/236832.png';
				}
			},
			error: error => {
				this.router.navigateByUrl('/login');
			},
		});
	}
}

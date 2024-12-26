import { Component, inject } from '@angular/core';
import { CouresRegistrationService } from '../coures-registration.service';
import { UserStore } from '../../stores/user.store';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';

@Component({
	selector: 'app-course-registration',
	standalone: true,
	imports: [RouterLink, FormsModule, CommonModule],
	templateUrl: './course-registration.component.html',
	styleUrl: './course-registration.component.scss',
})
export class CourseRegistrationComponent {
	courses: any = [];
	searchString: string | null = null;
	courseSearchs: any = [];

	loader = inject(LoaderModalStore);
	userStore = inject(UserStore);

	constructor(private couresRegistrationService: CouresRegistrationService) {}

	ngOnInit() {
		this.getCouresRegistrations();
	}

	private getCouresRegistrations() {
		patchState(this.loader, { isShow: true });
		this.couresRegistrationService.getCourses(this.userStore.studentId()).subscribe(data => {
			console.log(data);
			patchState(this.loader, { isShow: false });
			this.courses = data;
			this.courseSearchs = data;
		});
	}

	searchCourse() {
		this.courseSearchs = this.courses.filter((c: any) =>
			c.title.toLowerCase().includes(this.searchString)
		);
	}
}

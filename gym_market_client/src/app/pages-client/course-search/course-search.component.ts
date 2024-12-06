import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';

@Component({
	selector: 'app-course-search',
	standalone: true,
	imports: [RouterLink],
	templateUrl: './course-search.component.html',
	styleUrl: './course-search.component.scss',
})
export class CourseSearchComponent {
	courses: any;
	loader = inject(LoaderModalStore);

	constructor(private courseService: CourseAgencyService) {}

	ngOnInit() {
		this.getCourses();
	}

	private getCourses() {
		patchState(this.loader, { isShow: true });
		this.courseService.getCourses().subscribe({
			next: (res: any) => {
				// console.log(res);
				this.courses = res;
				patchState(this.loader, { isShow: false });
			},
		});
	}

	onSubmit() {
		console.log(123);
	}
}

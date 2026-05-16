import { Component, DestroyRef, inject } from '@angular/core';
import { CourseRegistrationService } from '../course-registration.service';
import { UserStore } from '../../stores/user.store';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Course } from '../../core/models/course.model';
import { GmButtonComponent } from '../../shared/components/gm-button/gm-button.component';

@Component({
	selector: 'app-course-registration',
	standalone: true,
	imports: [RouterLink, FormsModule, CommonModule, GmButtonComponent],
	templateUrl: './course-registration.component.html',
	styleUrl: './course-registration.component.scss',
})
export class CourseRegistrationComponent {
	courses: Course[] = [];
	searchString: string | null = null;
	courseSearchs: Course[] = [];

	loader = inject(LoaderModalStore);
	userStore = inject(UserStore);
	destroyRef = inject(DestroyRef);

	constructor(private courseRegistrationService: CourseRegistrationService) {}

	ngOnInit() {
		this.getCouresRegistrations();
	}

	private getCouresRegistrations() {
		patchState(this.loader, { isShow: true });
		this.courseRegistrationService
			.getCourses(this.userStore.studentId())
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe(data => {
				patchState(this.loader, { isShow: false });
				this.courses = data as unknown as Course[];
				this.courseSearchs = data as unknown as Course[];
			});
	}

	searchCourse() {
		if (this.searchString === null || this.searchString.trim() === '') {
			this.courseSearchs = this.courses;
			return;
		}
		this.courseSearchs = this.courses.filter((c: Course) =>
			c.title.toLowerCase().includes(this.searchString!.toLowerCase())
		);
	}
}

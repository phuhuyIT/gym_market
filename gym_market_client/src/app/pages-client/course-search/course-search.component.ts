import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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
	pageIndex: number = 1;
	pageSize: number = 10;

	constructor(
		private courseService: CourseAgencyService,
		private activatedRoute: ActivatedRoute,
        private router: Router
	) {}

	ngOnInit() {
		this.activatedRoute.queryParams.subscribe((res: any) => {
			this.pageIndex = res.pageIndex || 1;
			this.pageSize = res.pageSize || 10;
			this.getCourses();
		});
	}

	private getCourses() {
		patchState(this.loader, { isShow: true });
		this.courseService.getCourses(this.pageIndex, this.pageSize).subscribe({
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

	nexPage() {
		this.pageIndex++;
		this.router.navigate(['/client/course-search'], {queryParams: {pageIndex: this.pageIndex, pageSize: this.pageSize}});
	}

	prevPage() {
		this.pageIndex--;
		if (this.pageIndex < 1) {
			this.pageIndex = 1;
		}
		this.router.navigate(['/client/course-search'], {queryParams: {pageIndex: this.pageIndex, pageSize: this.pageSize}});
	}
}

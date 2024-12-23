import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
	selector: 'app-course-search',
	standalone: true,
	imports: [RouterLink, FormsModule, CommonModule],
	templateUrl: './course-search.component.html',
	styleUrl: './course-search.component.scss',
})
export class CourseSearchComponent {
	courses: any;
	loader = inject(LoaderModalStore);
	pageIndex: number = 1;
	pageSize: number = 10;
	searchString: string | null = null;
	category: string | null = '';

	categories = [
		'All',
		'Yoga',
		'Cardio',
		'Strength training',
		'Pilates',
		'Stretching',
		'Cross fit',
	];

	constructor(
		private courseService: CourseAgencyService,
		private activatedRoute: ActivatedRoute,
		private router: Router
	) {}

	ngOnInit() {
		this.activatedRoute.queryParams.subscribe((res: any) => {
			this.pageIndex = res.pageIndex || 1;
			this.pageSize = res.pageSize || 10;
			this.searchString = res.searchString || '';
			this.category = res.category || 'All';
			this.getCourses();
		});
	}

	private getCourses() {
		const category = this.category === 'All' ? '' : this.category;
		patchState(this.loader, { isShow: true });
		this.courseService
			.getCourses(this.pageIndex, this.pageSize, this.searchString, category)
			.subscribe({
				next: (res: any) => {
					// console.log(res);
					this.courses = res;
					patchState(this.loader, { isShow: false });
				},
			});
	}

	nexPage() {
		this.pageIndex++;
		this.onSubmit();
	}

	prevPage() {
		this.pageIndex--;
		if (this.pageIndex < 1) {
			this.pageIndex = 1;
		}
		this.onSubmit();
	}

	onSelectCategory(category: string) {
		this.category = category;
		this.onSubmit();
	}

	onSubmit() {
		this.router.navigate(['/client/course-search'], {
			queryParams: {
				pageIndex: this.pageIndex,
				pageSize: this.pageSize,
				searchString: this.searchString,
				category: this.category,
			},
		});
	}
}

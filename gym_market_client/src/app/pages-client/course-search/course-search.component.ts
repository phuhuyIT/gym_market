import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Course } from '../../core/models/course.model';
import { GmCardComponent } from '../../shared';

@Component({
    selector: 'app-course-search',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, FormsModule, CommonModule, GmCardComponent],
    templateUrl: './course-search.component.html',
    styleUrl: './course-search.component.scss'
})
export class CourseSearchComponent implements OnInit {
	courses: Course[] = [];
	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	pageIndex: number = 1;
	pageSize: number = 10;
	searchString: string = '';
	category: string = 'All';

	categories = [
		'All',
		'Yoga',
		'Cardio',
		'Strength training',
		'Pilates',
		'Stretching',
		'Cross fit',
	];

	bookmarkedCourses: Set<string> = new Set();

	constructor(
		private courseService: CourseAgencyService,
		private activatedRoute: ActivatedRoute,
		private router: Router
	) {}

	ngOnInit() {
		this.loadBookmarks();
		this.activatedRoute.queryParams
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe(params => {
				this.pageIndex = Number(params['pageIndex']) || 1;
				this.pageSize = Number(params['pageSize']) || 10;
				this.searchString = params['searchString'] || '';
				this.category = params['category'] || 'All';
				this.getCourses();
			});
	}

	private loadBookmarks() {
		const saved = localStorage.getItem('gym_bookmarked_courses');
		if (saved) {
			try {
				this.bookmarkedCourses = new Set(JSON.parse(saved));
			} catch (e) {
				console.error('Failed to parse bookmarks', e);
			}
		}
	}

	toggleBookmark(courseId: string, event: Event) {
		event.preventDefault();
		event.stopPropagation();
		if (this.bookmarkedCourses.has(courseId)) {
			this.bookmarkedCourses.delete(courseId);
		} else {
			this.bookmarkedCourses.add(courseId);
		}
		localStorage.setItem('gym_bookmarked_courses', JSON.stringify(Array.from(this.bookmarkedCourses)));
		this.cdr.markForCheck();
	}

	isBookmarked(courseId: string): boolean {
		return this.bookmarkedCourses.has(courseId);
	}

	getSpecialtyEmoji(category: string): string {
		const cat = (category || '').toLowerCase();
		if (cat.includes('yoga')) return '🧘';
		if (cat.includes('cardio') || cat.includes('run')) return '🏃';
		if (cat.includes('strength') || cat.includes('weight') || cat.includes('power')) return '💪';
		if (cat.includes('pilates')) return '🤸';
		if (cat.includes('stretch')) return '🙆';
		if (cat.includes('cross fit') || cat.includes('crossfit') || cat.includes('hiit')) return '🏋️';
		return '💪';
	}

	private getCourses() {
		const category = this.category === 'All' ? '' : this.category;
		patchState(this.loader, { isShow: true });
		this.courseService
			.getCourses(this.pageIndex, this.pageSize, this.searchString, category)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.courses = res;
					patchState(this.loader, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
				},
			});
	}

	nexPage() {
		this.pageIndex++;
		this.onSubmit();
	}

	prevPage() {
		if (this.pageIndex > 1) {
			this.pageIndex--;
			this.onSubmit();
		}
	}

	onSelectCategory(category: string) {
		this.category = category;
		this.pageIndex = 1; // Reset to first page on category change
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

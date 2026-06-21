import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { TrainerService } from '../../page-agency/trainer.service';
import { RouterLink } from '@angular/router';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Trainer, TrainerSearch } from '../../core/models/trainer.model';

import { FormsModule } from '@angular/forms';
import { GmCardComponent } from '../../shared/components/gm-card/gm-card.component';
import { SEARCH_DEBOUNCE_MS } from '../../utilities/defaults.const';
import { STORAGE_KEYS } from '../../utilities/storage-keys.const';
import { DEFAULT_AVATAR_IMAGE_URL } from '../../utilities/defaults.const';
import { FallbackSrcDirective } from '../../shared/directives/fallback-src.directive';
import { matchesSearch, normalizeSearch } from '../../shared/utils/search.util';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
    selector: 'app-trainer-list',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, FormsModule, GmCardComponent, FallbackSrcDirective],
    templateUrl: './trainer-list.component.html',
    styleUrl: './trainer-list.component.scss'
})
export class TrainerListComponent implements OnInit {
	trainers: Trainer[] = [];
	searchString: string = '';
	selectedCategory: string = 'All';
	categories: string[] = ['All', 'Yoga', 'Cardio', 'Strength', 'Crossfit', 'Elite'];
	bookmarkedTrainers: Set<string> = new Set();
	readonly DEFAULT_AVATAR_IMAGE_URL = DEFAULT_AVATAR_IMAGE_URL;
	pageIndex = 1;
	pageSize = 12;
	totalCount = 0;
	totalPages = 0;
	hasPreviousPage = false;
	hasNextPage = false;

	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private searchChanged$ = new Subject<string>();

	constructor(private trainerService: TrainerService) {}

	ngOnInit() {
		this.searchChanged$
			.pipe(
				debounceTime(SEARCH_DEBOUNCE_MS),
				distinctUntilChanged(),
				takeUntilDestroyed(this.destroyRef)
			)
			.subscribe(() => {
				this.pageIndex = 1;
				this.getAllTrainers();
			});

		this.getAllTrainers();
		this.loadBookmarks();
	}

	private loadBookmarks() {
		const saved = localStorage.getItem(STORAGE_KEYS.bookmarkedTrainers);
		if (saved) {
			try {
				this.bookmarkedTrainers = new Set(JSON.parse(saved));
			} catch {
				localStorage.removeItem(STORAGE_KEYS.bookmarkedTrainers);
			}
		}
	}

	private getAllTrainers() {
		patchState(this.loader, { isShow: true });
		this.trainerService
			.searchTrainersPaged(this.searchString, this.pageIndex, this.pageSize)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					patchState(this.loader, { isShow: false });
					this.trainers = res.items.map(trainer => this.toTrainer(trainer));
					this.totalCount = res.totalCount;
					this.totalPages = res.totalPages;
					this.hasPreviousPage = res.hasPreviousPage;
					this.hasNextPage = res.hasNextPage;
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
				},
			});
	}

	private toTrainer(trainer: TrainerSearch): Trainer {
		return {
			trainerId: trainer.trainerId,
			userId: trainer.userId || '',
			name: trainer.fullName || trainer.name || 'Trainer',
			email: trainer.email || '',
			profilePicture: trainer.profilePicture || '',
			bio: trainer.bio || '',
			certification: trainer.certification || '',
			experience: trainer.experience || 0,
			rating: trainer.rating || 0,
			createdAt: trainer.createdAt || '',
			updatedAt: trainer.createdAt || ''
		};
	}

	get filteredTrainers(): Trainer[] {
		const selectedCategory = normalizeSearch(this.selectedCategory);
		return this.trainers.filter(trainer => {
			const matchesText = matchesSearch(this.searchString, [trainer.name, trainer.email, trainer.certification, trainer.bio]);

			const matchesCategory = this.selectedCategory === 'All' ||
				(this.selectedCategory === 'Elite' && trainer.experience >= 8) ||
				normalizeSearch(trainer.certification).includes(selectedCategory) ||
				normalizeSearch(trainer.bio).includes(selectedCategory);

			return matchesText && matchesCategory;
		});
	}

	selectCategory(category: string) {
		this.selectedCategory = category;
		this.cdr.markForCheck();
	}

	goToPage(pageIndex: number) {
		if (pageIndex < 1 || (this.totalPages && pageIndex > this.totalPages) || pageIndex === this.pageIndex) return;
		this.pageIndex = pageIndex;
		this.getAllTrainers();
	}

	toggleBookmark(trainerId: string, event: Event) {
		event.preventDefault();
		event.stopPropagation();
		if (this.bookmarkedTrainers.has(trainerId)) {
			this.bookmarkedTrainers.delete(trainerId);
		} else {
			this.bookmarkedTrainers.add(trainerId);
		}
		localStorage.setItem(STORAGE_KEYS.bookmarkedTrainers, JSON.stringify(Array.from(this.bookmarkedTrainers)));
		this.cdr.markForCheck();
	}

	isBookmarked(trainerId: string): boolean {
		return this.bookmarkedTrainers.has(trainerId);
	}

	getReviewsCount(trainer: Trainer): number {
		const hash = trainer.name.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0);
		return (hash % 120) + 15;
	}

	getClientsCount(trainer: Trainer): number {
		const hash = trainer.name.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0);
		return (hash % 35) + 5;
	}

	getSpecialtyEmoji(trainer: Trainer): string {
		const cert = (trainer.certification || '').toLowerCase();
		const bio = (trainer.bio || '').toLowerCase();
		if (cert.includes('yoga') || bio.includes('yoga') || bio.includes('wellness')) return '🧘';
		if (cert.includes('cardio') || cert.includes('run') || bio.includes('cardio') || bio.includes('run') || bio.includes('cycling')) return '🏃';
		if (cert.includes('strength') || cert.includes('weight') || bio.includes('strength') || bio.includes('weight') || bio.includes('muscle')) return '💪';
		if (cert.includes('crossfit') || cert.includes('lift') || bio.includes('crossfit') || bio.includes('lift') || bio.includes('power')) return '🏋️';
		if (cert.includes('boxing') || cert.includes('fight') || bio.includes('boxing') || bio.includes('fight')) return '🥊';
		if (cert.includes('hiit') || cert.includes('interval') || bio.includes('hiit') || bio.includes('interval')) return '⚡';
		return '💪';
	}

	onSubmit() {
		this.searchChanged$.next(this.searchString);
	}
}

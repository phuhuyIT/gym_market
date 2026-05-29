import {
	ChangeDetectionStrategy,
	ChangeDetectorRef,
	Component,
	DestroyRef,
	EventEmitter,
	inject,
	OnInit,
	Output,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { ConversationService } from '../conversation.service';
import { UserSearchResult } from '../../core/models/conversation.model';
import { DEFAULT_AVATAR_URL } from '../../utilities/defaults.const';

@Component({
	selector: 'app-create-group',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [FormsModule],
	templateUrl: './create-group.component.html',
	styleUrl: './create-group.component.scss',
})
export class CreateGroupComponent implements OnInit {
	@Output() close = new EventEmitter<void>();
	@Output() created = new EventEmitter<void>();

	readonly DEFAULT_AVATAR_URL = DEFAULT_AVATAR_URL;

	groupName = '';
	searchQuery = '';
	results: UserSearchResult[] = [];
	selected: UserSearchResult[] = [];
	submitting = false;
	error = '';

	private searchTerm$ = new Subject<string>();
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private conversationService = inject(ConversationService);

	ngOnInit() {
		this.searchTerm$
			.pipe(
				debounceTime(300),
				distinctUntilChanged(),
				switchMap(q => this.conversationService.searchUsers(q)),
				takeUntilDestroyed(this.destroyRef)
			)
			.subscribe({
				next: res => {
					this.results = res;
					this.cdr.markForCheck();
				},
			});
	}

	onSearchChange(value: string) {
		this.searchQuery = value;
		this.searchTerm$.next(value);
	}

	isSelected(user: UserSearchResult): boolean {
		return this.selected.some(u => u.id === user.id);
	}

	toggleMember(user: UserSearchResult) {
		if (this.isSelected(user)) {
			this.selected = this.selected.filter(u => u.id !== user.id);
		} else {
			this.selected = [...this.selected, user];
		}
	}

	get canCreate(): boolean {
		return this.groupName.trim().length > 0 && this.selected.length > 0 && !this.submitting;
	}

	createGroup() {
		if (!this.canCreate) {
			return;
		}
		this.submitting = true;
		this.error = '';
		this.conversationService
			.createGroup({
				name: this.groupName.trim(),
				memberIds: this.selected.map(u => u.id),
			})
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.submitting = false;
					this.created.emit();
				},
				error: () => {
					this.submitting = false;
					this.error = 'Could not create the group. Please try again.';
					this.cdr.markForCheck();
				},
			});
	}

	onClose() {
		this.close.emit();
	}
}

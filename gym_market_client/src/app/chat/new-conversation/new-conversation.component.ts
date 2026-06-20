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
import { UserStore } from '../../stores/user.store';
import { DEFAULT_AVATAR_IMAGE_URL } from '../../utilities/defaults.const';
import { FallbackSrcDirective } from '../../shared/directives/fallback-src.directive';

@Component({
	selector: 'app-new-conversation',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [FormsModule, FallbackSrcDirective],
	templateUrl: './new-conversation.component.html',
	styleUrl: './new-conversation.component.scss',
})
export class NewConversationComponent implements OnInit {
	@Output() close = new EventEmitter<void>();
	@Output() created = new EventEmitter<void>();

	readonly DEFAULT_AVATAR_IMAGE_URL = DEFAULT_AVATAR_IMAGE_URL;

	searchQuery = '';
	results: UserSearchResult[] = [];
	submitting = false;
	error = '';

	private searchTerm$ = new Subject<string>();
	private userStore = inject(UserStore);
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

	startConversation(user: UserSearchResult) {
		if (this.submitting) {
			return;
		}
		const senderId = this.userStore.id();
		if (!senderId) {
			return;
		}
		this.submitting = true;
		this.error = '';
		this.conversationService
			.createConversation({ recieveId: user.id })
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.submitting = false;
					this.created.emit();
				},
				error: () => {
					this.submitting = false;
					this.error = 'Could not start the conversation. Please try again.';
					this.cdr.markForCheck();
				},
			});
	}

	onClose() {
		this.close.emit();
	}
}

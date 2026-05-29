import {
	ChangeDetectionStrategy,
	ChangeDetectorRef,
	Component,
	DestroyRef,
	EventEmitter,
	inject,
	Input,
	OnInit,
	Output,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { ConversationService } from '../conversation.service';
import { GroupMember, UserSearchResult } from '../../core/models/conversation.model';
import { UserStore } from '../../stores/user.store';
import { DEFAULT_AVATAR_URL } from '../../utilities/defaults.const';

@Component({
	selector: 'app-group-members',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [FormsModule],
	templateUrl: './group-members.component.html',
	styleUrl: './group-members.component.scss',
})
export class GroupMembersComponent implements OnInit {
	@Input({ required: true }) conversationId!: number;
	@Input() groupName = '';

	@Output() close = new EventEmitter<void>();
	@Output() left = new EventEmitter<void>();
	@Output() changed = new EventEmitter<void>();

	readonly DEFAULT_AVATAR_URL = DEFAULT_AVATAR_URL;

	members: GroupMember[] = [];
	myRole = 'Member';

	showAdd = false;
	searchQuery = '';
	results: UserSearchResult[] = [];

	private searchTerm$ = new Subject<string>();
	userStore = inject(UserStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private conversationService = inject(ConversationService);

	get isOwner(): boolean {
		return this.myRole === 'Owner';
	}

	get canManage(): boolean {
		return this.myRole === 'Owner' || this.myRole === 'Admin';
	}

	ngOnInit() {
		this.loadMembers();

		this.searchTerm$
			.pipe(
				debounceTime(300),
				distinctUntilChanged(),
				switchMap(q => this.conversationService.searchUsers(q)),
				takeUntilDestroyed(this.destroyRef)
			)
			.subscribe({
				next: res => {
					const existingIds = new Set(this.members.map(m => m.userId));
					this.results = res.filter(u => !existingIds.has(u.id));
					this.cdr.markForCheck();
				},
			});
	}

	private loadMembers() {
		this.conversationService
			.getGroupMembers(this.conversationId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.members = res;
					this.myRole = res.find(m => m.userId === this.userStore.id())?.role ?? 'Member';
					this.cdr.markForCheck();
				},
			});
	}

	onSearchChange(value: string) {
		this.searchQuery = value;
		this.searchTerm$.next(value);
	}

	addMember(user: UserSearchResult) {
		this.conversationService
			.addMembers({ conversationId: this.conversationId, userIds: [user.id] })
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.results = this.results.filter(u => u.id !== user.id);
					this.loadMembers();
					this.changed.emit();
				},
			});
	}

	removeMember(member: GroupMember) {
		this.conversationService
			.removeMember(this.conversationId, member.userId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.loadMembers();
					this.changed.emit();
				},
			});
	}

	setRole(member: GroupMember, role: string) {
		this.conversationService
			.updateMemberRole({ conversationId: this.conversationId, userId: member.userId, role })
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.loadMembers();
					this.changed.emit();
				},
			});
	}

	leaveGroup() {
		this.conversationService
			.leaveGroup(this.conversationId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.left.emit();
				},
			});
	}

	isSelf(member: GroupMember): boolean {
		return member.userId === this.userStore.id();
	}

	onClose() {
		this.close.emit();
	}
}

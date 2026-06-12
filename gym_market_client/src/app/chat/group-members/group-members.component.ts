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
import { debounceTime, distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { ConversationService } from '../conversation.service';
import { GroupMember, UserSearchResult } from '../../core/models/conversation.model';
import { UserStore } from '../../stores/user.store';
import { DEFAULT_AVATAR_URL, DEFAULT_GROUP_AVATAR_URL } from '../../utilities/defaults.const';

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
	@Input() groupAvatar = '';

	@Output() close = new EventEmitter<void>();
	@Output() left = new EventEmitter<void>();
	@Output() changed = new EventEmitter<void>();

	readonly DEFAULT_AVATAR_URL = DEFAULT_AVATAR_URL;
	readonly DEFAULT_GROUP_AVATAR_URL = DEFAULT_GROUP_AVATAR_URL;

	members: GroupMember[] = [];
	myRole = 'Member';

	editingName = false;
	editName = '';
	savingName = false;
	uploadingAvatar = false;
	groupError = '';

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

	startEditName() {
		this.editName = this.groupName;
		this.editingName = true;
	}

	cancelEditName() {
		this.editingName = false;
		this.groupError = '';
	}

	saveName() {
		const name = this.editName.trim();
		if (!name || this.savingName) {
			return;
		}
		if (name === this.groupName) {
			this.editingName = false;
			return;
		}
		this.savingName = true;
		this.groupError = '';
		this.conversationService
			.updateGroup({ conversationId: this.conversationId, name })
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.savingName = false;
					this.editingName = false;
					this.groupName = name;
					this.changed.emit();
					this.cdr.markForCheck();
				},
				error: () => {
					this.savingName = false;
					this.groupError = 'Could not rename the group. Please try again.';
					this.cdr.markForCheck();
				},
			});
	}

	onAvatarSelected(event: Event) {
		const input = event.target as HTMLInputElement;
		const file = input.files?.[0];
		input.value = '';
		if (!file || this.uploadingAvatar) {
			return;
		}
		if (!file.type.startsWith('image/')) {
			this.groupError = 'Please choose an image file.';
			return;
		}
		if (file.size > 5 * 1024 * 1024) {
			this.groupError = 'Image must be smaller than 5 MB.';
			return;
		}
		this.uploadingAvatar = true;
		this.groupError = '';
		this.conversationService
			.uploadGroupAvatar(file)
			.pipe(
				switchMap(res =>
					this.conversationService.updateGroup({
						conversationId: this.conversationId,
						avatarUrl: res.avatarUrl,
					}).pipe(map(() => res.avatarUrl))
				),
				takeUntilDestroyed(this.destroyRef)
			)
			.subscribe({
				next: avatarUrl => {
					this.uploadingAvatar = false;
					if (avatarUrl) {
						this.groupAvatar = avatarUrl;
					}
					this.changed.emit();
					this.cdr.markForCheck();
				},
				error: () => {
					this.uploadingAvatar = false;
					this.groupError = 'Could not update the group photo. Please try again.';
					this.cdr.markForCheck();
				},
			});
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

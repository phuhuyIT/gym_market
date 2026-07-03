import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CourseAnnouncement, UpsertCourseAnnouncement } from '../../core/models/announcement.model';
import { AnnouncementService } from '../../core/services/announcement.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-course-announcements',
	standalone: true,
	imports: [CommonModule, FormsModule, RouterLink],
	templateUrl: './course-announcements.component.html',
	styleUrl: './course-announcements.component.scss',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CourseAnnouncementsComponent implements OnInit {
	courseId = '';
	announcements: CourseAnnouncement[] = [];
	selected: CourseAnnouncement | null = null;
	title = '';
	body = '';
	isPinned = false;
	isPublished = true;
	isLoading = false;
	isSaving = false;

	private route = inject(ActivatedRoute);
	private announcementService = inject(AnnouncementService);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit(): void {
		this.courseId = this.route.snapshot.params['courseId'];
		this.loadAnnouncements();
	}

	loadAnnouncements(): void {
		this.isLoading = true;
		this.announcementService
			.getCourseAnnouncements(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: announcements => {
					this.announcements = announcements;
					this.isLoading = false;
					if (this.selected) {
						this.selected = announcements.find(item => item.announcementId === this.selected?.announcementId) ?? null;
					}
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load announcements', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	selectAnnouncement(announcement: CourseAnnouncement): void {
		this.selected = announcement;
		this.title = announcement.title;
		this.body = announcement.body;
		this.isPinned = announcement.isPinned;
		this.isPublished = announcement.isPublished;
	}

	newAnnouncement(): void {
		this.selected = null;
		this.title = '';
		this.body = '';
		this.isPinned = false;
		this.isPublished = true;
	}

	save(): void {
		if (this.isSaving) return;
		if (!this.title.trim() || !this.body.trim()) {
			this.toastService.show('Title and message are required', 'error');
			return;
		}

		const model: UpsertCourseAnnouncement = {
			title: this.title.trim(),
			body: this.body.trim(),
			isPinned: this.isPinned,
			isPublished: this.isPublished,
		};

		this.isSaving = true;
		const request = this.selected
			? this.announcementService.updateAnnouncement(this.selected.announcementId, model)
			: this.announcementService.createAnnouncement(this.courseId, model);

		request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: announcement => {
				this.isSaving = false;
				this.selected = announcement;
				this.toastService.show(announcement.isPublished ? 'Announcement published' : 'Announcement saved');
				this.loadAnnouncements();
				this.cdr.markForCheck();
			},
			error: err => {
				this.isSaving = false;
				this.toastService.show(err?.error?.message || err?.error?.Message || 'Failed to save announcement', 'error');
				this.cdr.markForCheck();
			},
		});
	}

	togglePublished(announcement: CourseAnnouncement): void {
		this.updateQuick(announcement, { isPublished: !announcement.isPublished });
	}

	togglePinned(announcement: CourseAnnouncement): void {
		this.updateQuick(announcement, { isPinned: !announcement.isPinned });
	}

	deleteAnnouncement(announcement: CourseAnnouncement): void {
		this.announcementService
			.deleteAnnouncement(announcement.announcementId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					if (this.selected?.announcementId === announcement.announcementId) this.newAnnouncement();
					this.toastService.show('Announcement deleted');
					this.loadAnnouncements();
				},
				error: () => this.toastService.show('Failed to delete announcement', 'error'),
			});
	}

	statusLabel(announcement: CourseAnnouncement): string {
		return announcement.isPublished ? 'Published' : 'Draft';
	}

	private updateQuick(announcement: CourseAnnouncement, changes: Partial<UpsertCourseAnnouncement>): void {
		this.announcementService
			.updateAnnouncement(announcement.announcementId, {
				title: announcement.title,
				body: announcement.body,
				isPinned: changes.isPinned ?? announcement.isPinned,
				isPublished: changes.isPublished ?? announcement.isPublished,
			})
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: updated => {
					this.selected = this.selected?.announcementId === updated.announcementId ? updated : this.selected;
					this.toastService.show('Announcement updated');
					this.loadAnnouncements();
				},
				error: () => this.toastService.show('Failed to update announcement', 'error'),
			});
	}
}

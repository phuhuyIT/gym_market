import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CourseAnnouncement } from '../../core/models/announcement.model';
import { AnnouncementService } from '../../core/services/announcement.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-client-course-announcements',
	standalone: true,
	imports: [CommonModule, RouterLink],
	templateUrl: './course-announcements.component.html',
	styleUrl: './course-announcements.component.scss',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CourseAnnouncementsComponent implements OnInit {
	courseId = '';
	announcements: CourseAnnouncement[] = [];
	isLoading = false;

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
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load announcements', 'error');
					this.cdr.markForCheck();
				},
			});
	}
}

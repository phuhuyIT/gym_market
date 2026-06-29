import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { PagedResult } from '../../core/models/paged-result.model';
import { AdminNotificationTemplate, NotificationDeliveryLog, UpdateNotificationTemplate } from './admin-notification.model';

@Injectable({
	providedIn: 'root',
})
export class AdminNotificationsService {
	private readonly base = `${environment.baseApi}/Admin/notifications`;

	constructor(private http: HttpClient) {}

	getTemplates(): Observable<AdminNotificationTemplate[]> {
		return this.http.get<AdminNotificationTemplate[]>(`${this.base}/templates`);
	}

	updateTemplate(type: string, model: UpdateNotificationTemplate): Observable<AdminNotificationTemplate> {
		return this.http.put<AdminNotificationTemplate>(`${this.base}/templates/${encodeURIComponent(type)}`, model);
	}

	getDeliveries(params: {
		type?: string;
		channel?: string;
		status?: string;
		pageIndex?: number;
		pageSize?: number;
	}): Observable<PagedResult<NotificationDeliveryLog>> {
		const query: Record<string, string> = {
			pageIndex: String(params.pageIndex ?? 1),
			pageSize: String(params.pageSize ?? 25),
		};

		if (params.type) query['type'] = params.type;
		if (params.channel) query['channel'] = params.channel;
		if (params.status) query['status'] = params.status;

		return this.http.get<PagedResult<NotificationDeliveryLog>>(`${this.base}/deliveries`, { params: query });
	}
}

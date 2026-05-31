import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface AuditLogDto {
  id: number;
  tableName: string;
  action: string;
  recordId: number | null;
  oldValues: string | null;
  newValues: string | null;
  changedBy: string | null;
  changedAt: string;
}

export interface PaginatedAuditLogs {
  items: AuditLogDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface AuditLogFilter {
  tableName?: string;
  action?: string;
  from?: string;
  to?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class AuditLogService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/auditlogs`;

  getAll(filter?: AuditLogFilter) {
    let params = new HttpParams();
    if (filter) {
      if (filter.tableName) params = params.set('tableName', filter.tableName);
      if (filter.action) params = params.set('action', filter.action);
      if (filter.from) params = params.set('from', filter.from);
      if (filter.to) params = params.set('to', filter.to);
      if (filter.page) params = params.set('page', filter.page);
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize);
    }
    params = params.set('_', Date.now().toString());
    return this.http.get<PaginatedAuditLogs>(this.API, { params });
  }
}

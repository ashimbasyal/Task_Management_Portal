import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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

@Injectable({ providedIn: 'root' })
export class AuditLogService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/auditlogs`;

  getAll() {
    return this.http.get<AuditLogDto[]>(this.API + '?_=' + Date.now());
  }
}

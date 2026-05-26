import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

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
  private readonly API = 'http://localhost:5072/api/auditlogs';

  getAll() {
    return this.http.get<AuditLogDto[]>(this.API + '?_=' + Date.now());
  }
}

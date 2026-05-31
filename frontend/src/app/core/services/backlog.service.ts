import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface BacklogItem {
  id: number;
  sn: number;
  title: string;
  description: string | null;
  requestedBy: string;
  gitLabLink: string | null;
  remarks: string | null;
  priorityId: number | null;
  priorityName: string | null;
  statusId: number | null;
  statusName: string | null;
  departmentId: number | null;
  departmentName: string | null;
  isMovedToSprint: boolean;
  createdAt: string;
  createdBy: string | null;
  updatedAt: string | null;
  updatedBy: string | null;
}

export interface CreateBacklogRequest {
  title: string;
  description: string | null;
  requestedBy: string;
  gitLabLink: string | null;
  remarks: string | null;
  priorityId: number | null;
  statusId: number | null;
  departmentId: number | null;
}

export interface UpdateBacklogRequest {
  title: string;
  description: string | null;
  requestedBy: string;
  gitLabLink: string | null;
  remarks: string | null;
  priorityId: number | null;
  statusId: number | null;
  departmentId: number | null;
}

@Injectable({ providedIn: 'root' })
export class BacklogService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/backlogtask`;

  getAll(priorityId?: number | null, statusId?: number | null, departmentId?: number | null) {
    let params = new HttpParams();
    if (priorityId != null) params = params.set('priorityId', priorityId);
    if (statusId != null) params = params.set('statusId', statusId);
    if (departmentId != null) params = params.set('departmentId', departmentId);
    return this.http.get<any>(this.API, { params }).pipe(
      map(res => res.data as BacklogItem[])
    );
  }

  create(request: CreateBacklogRequest) {
    return this.http.post<any>(this.API, request).pipe(
      map(res => res.data || { id: 0, title: request.title })
    );
  }

  update(id: number, request: UpdateBacklogRequest) {
    return this.http.put<any>(`${this.API}/${id}`, request).pipe(
      map(res => res.data || { id })
    );
  }

  delete(id: number) {
    return this.http.delete<any>(`${this.API}/${id}`).pipe(
      map(res => res)
    );
  }

  downloadSample() {
    return this.http.get(`${this.API}/download-sample`, { responseType: 'blob' });
  }

  bulkUpload(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<any>(`${this.API}/bulk-upload`, formData).pipe(
      map(res => res)
    );
  }
}

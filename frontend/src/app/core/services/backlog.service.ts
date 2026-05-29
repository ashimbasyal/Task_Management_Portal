import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface BacklogItem {
  id: number;
  title: string;
  description: string | null;
  requestedBy: string;
  gitLabLink: string | null;
  remarks: string | null;
  priority: string;
  status: string;
  department: string;
  isMovedToSprint: boolean;
}

export interface CreateBacklogRequest {
  title: string;
  description: string | null;
  requestedBy: string;
  gitLabLink: string | null;
  remarks: string | null;
  priority: string;
  status: string;
  department: string;
}

export interface MoveToSprintRequest {
  sprintName: string;
  startDate: string | null;
  endDate: string | null;
  remarks: string | null;
  assigneeId: number | null;
}

@Injectable({ providedIn: 'root' })
export class BacklogService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/backlog`;

  getAll() {
    return this.http.get<BacklogItem[]>(this.API + '?_=' + Date.now());
  }

  create(request: CreateBacklogRequest) {
    return this.http.post<{ id: number }>(this.API, request);
  }

  moveToSprint(id: number, request: MoveToSprintRequest) {
    return this.http.post<{ sprintId: number }>(`${this.API}/${id}/move-to-sprint`, request);
  }
}

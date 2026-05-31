import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface SprintTaskDto {
  id: number;
  backlogTaskId: number;
  backlogTaskSN: number | null;
  backlogTaskTitle: string | null;
  sprintName: string | null;
  startDate: string | null;
  endDate: string | null;
  remarks: string | null;
  assigneeId: string | null;
  assigneeName: string | null;
  statusId: number | null;
  statusName: string | null;
  createdAt: string;
  createdBy: string | null;
  updatedAt: string | null;
  updatedBy: string | null;
}

export interface CreateSprintTaskRequest {
  backlogTaskId: number;
  sprintName: string;
  startDate: string | null;
  endDate: string | null;
  remarks: string | null;
  assigneeId: string | null;
}

export interface UpdateSprintTaskRequest {
  sprintName: string | null;
  startDate: string | null;
  endDate: string | null;
  remarks: string | null;
  assigneeId: string | null;
  statusId: number | null;
}

@Injectable({ providedIn: 'root' })
export class SprintTaskService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/sprinttasks`;

  getAll() {
    return this.http.get<any>(this.API).pipe(
      map(res => res.data as SprintTaskDto[])
    );
  }

  getById(id: number) {
    return this.http.get<any>(`${this.API}/${id}`).pipe(
      map(res => res.data as SprintTaskDto)
    );
  }

  create(request: CreateSprintTaskRequest) {
    return this.http.post<any>(this.API, request).pipe(
      map(res => res.data || { id: 0 })
    );
  }

  update(id: number, request: UpdateSprintTaskRequest) {
    return this.http.put<any>(this.API, { id, ...request }).pipe(
      map(res => res)
    );
  }

  delete(id: number) {
    return this.http.delete<any>(`${this.API}/${id}`).pipe(
      map(res => res)
    );
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface SprintStatusTriggerDto {
  id: number;
  name: string;
}

export interface CreateSprintStatusTriggerRequest {
  name: string;
}

export interface UpdateSprintStatusTriggerRequest {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class SprintStatusTriggerService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/sprintstatustrigger`;

  getAll() {
    return this.http.get<any>(this.API).pipe(
      map(res => res.data as SprintStatusTriggerDto[])
    );
  }

  create(request: CreateSprintStatusTriggerRequest) {
    return this.http.post<any>(this.API, request).pipe(
      map(res => res.data)
    );
  }

  update(id: number, request: UpdateSprintStatusTriggerRequest) {
    return this.http.put<any>(this.API, request).pipe(
      map(res => res)
    );
  }

  delete(id: number) {
    return this.http.delete<any>(`${this.API}/${id}`).pipe(
      map(res => res)
    );
  }
}

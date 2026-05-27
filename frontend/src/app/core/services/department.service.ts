import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface DepartmentDto {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class DepartmentService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/departments`;

  getAll() {
    return this.http.get<DepartmentDto[]>(this.API);
  }
}

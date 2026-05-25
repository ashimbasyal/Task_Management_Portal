import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface DepartmentDto {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class DepartmentService {
  private http = inject(HttpClient);
  private readonly API = 'http://localhost:5072/api/departments';

  getAll() {
    return this.http.get<DepartmentDto[]>(this.API);
  }
}

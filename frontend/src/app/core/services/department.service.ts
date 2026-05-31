import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
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
    return this.http.get<any>(this.API).pipe(
      map(res => res.data as DepartmentDto[])
    );
  }

  create(data: { name: string }) {
    return this.http.post<any>(this.API, data).pipe(
      map(res => res.data)
    );
  }

  update(id: number, data: { name: string }) {
    return this.http.put<any>(this.API, { id, ...data }).pipe(
      map(res => res.data)
    );
  }

  delete(id: number) {
    return this.http.delete<any>(`${this.API}/${id}`).pipe(
      map(res => res)
    );
  }
}

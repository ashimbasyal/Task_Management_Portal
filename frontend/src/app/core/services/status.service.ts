import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface StatusDto {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class StatusService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/status`;

  getAll() {
    return this.http.get<any>(this.API).pipe(
      map(res => res.data as StatusDto[])
    );
  }
}

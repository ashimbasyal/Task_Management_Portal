import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface MasterDataEntry {
  id: number;
  type: number;
  value: string;
  displayOrder: number;
}

export interface CreateMasterDataRequest {
  type: number;
  value: string;
  displayOrder: number;
}

@Injectable({ providedIn: 'root' })
export class MasterDataService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/masterdata`;

  getByType(type: number) {
    return this.http.get<any>(`${this.API}/${type}`).pipe(
      map(res => res.data as MasterDataEntry[])
    );
  }

  create(request: CreateMasterDataRequest) {
    return this.http.post<any>(this.API, request).pipe(
      map(res => res.data || { id: 0 })
    );
  }

  update(id: number, request: { value?: string; displayOrder: number }) {
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

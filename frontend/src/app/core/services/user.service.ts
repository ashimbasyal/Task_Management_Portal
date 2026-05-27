import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface UserDto {
  id: string;
  fullName: string;
  email: string;
  role: number;
  departmentId: number | null;
  departmentName: string | null;
  canViewAllDepartments: boolean;
}

export interface CreateUserRequest {
  fullName: string;
  email: string;
  password: string;
  role: number;
  departmentId: number | null;
}

export interface UpdatePermissionRequest {
  canViewAllDepartments: boolean;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/users`;

  getAll() {
    return this.http.get<UserDto[]>(this.API);
  }

  getById(id: string) {
    return this.http.get<UserDto>(`${this.API}/${id}`);
  }

  create(data: CreateUserRequest) {
    return this.http.post<UserDto>(this.API, data);
  }

  updatePermission(id: string, data: UpdatePermissionRequest) {
    return this.http.patch<UserDto>(`${this.API}/${id}/permission`, data);
  }

  delete(id: string) {
    return this.http.delete(`${this.API}/${id}`, { responseType: 'text' });
  }
}

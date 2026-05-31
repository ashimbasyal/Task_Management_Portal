import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Permission } from './permission.enum';
import { RolePermissions } from './role-permissions';
import { environment } from '../../../environments/environment';
import { ActivityTrackerService } from './activity-tracker.service';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

interface TokenPayload {
  sub: string;
  email: string;
  name: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  Permission?: string | string[];
  DepartmentId?: string;
  exp: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private activity = inject(ActivityTrackerService);
  private readonly API = environment.apiBaseUrl;
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_KEY = 'refresh_token';

  isLoggedIn = signal(!!localStorage.getItem(this.TOKEN_KEY));
  /** tracks an in-flight refresh so concurrent 401s share one attempt */
  private refreshInProgress: Promise<void> | null = null;
  private refreshTimer: ReturnType<typeof setInterval> | null = null;

  constructor() {
    if (this.isLoggedIn()) {
      this.startSessionKeepAlive();
    }
  }

  private startSessionKeepAlive() {
    this.activity.start();
    this.scheduleRefresh();
  }

  private scheduleRefresh() {
    if (this.refreshTimer) clearInterval(this.refreshTimer);
    this.refreshTimer = setInterval(() => {
      if (this.activity.isActive()) {
        this.refresh().catch(() => {});
      }
    }, 4 * 60 * 1000);
  }

  login(email: string, password: string) {
    return this.http.post<AuthResponse>(`${this.API}/auth/login`, { email, password }).pipe(
      tap(res => {
        this.setTokens(res);
        this.startSessionKeepAlive();
      })
    );
  }

  register(email: string, password: string) {
    return this.http.post<AuthResponse>(`${this.API}/auth/register`, { email, password }).pipe(
      tap(res => {
        this.setTokens(res);
        this.startSessionKeepAlive();
      })
    );
  }

  refresh(): Promise<void> {
    if (this.refreshInProgress) return this.refreshInProgress;
    const refreshToken = localStorage.getItem(this.REFRESH_KEY);
    if (!refreshToken) {
      this.logout();
      return Promise.reject('No refresh token');
    }
    this.refreshInProgress = new Promise((resolve, reject) => {
      this.http.post<AuthResponse>(`${this.API}/auth/refresh`, { refreshToken }).subscribe({
        next: res => {
          this.setTokens(res);
          this.refreshInProgress = null;
          resolve();
        },
        error: err => {
          this.refreshInProgress = null;
          this.logout();
          reject(err);
        }
      });
    });
    return this.refreshInProgress;
  }

  logout() {
    if (this.refreshTimer) {
      clearInterval(this.refreshTimer);
      this.refreshTimer = null;
    }
    this.activity.stop();
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_KEY);
    this.isLoggedIn.set(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private getPayload(): TokenPayload | null {
    const token = this.getToken();
    if (!token) return null;
    try {
      return JSON.parse(atob(token.split('.')[1]));
    } catch { return null; }
  }

  getRole(): string {
    const payload = this.getPayload();
    if (!payload) return '';
    return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? '';
  }

  getPermissions(): Permission[] {
    const payload = this.getPayload();
    if (!payload) return [];

    const raw = payload.Permission;
    if (Array.isArray(raw)) return raw as Permission[];
    if (typeof raw === 'string') return [raw as Permission];

    const roleNum = this.getRoleNumber();
    return RolePermissions[roleNum] ?? [];
  }

  getRoleNumber(): number {
    const roleMap: Record<string, number> = { Admin: 1, Developer: 2, Officer: 3 };
    return roleMap[this.getRole()] ?? 0;
  }

  hasPermission(permission: Permission): boolean {
    return this.getPermissions().includes(permission);
  }

  getDepartmentId(): number | null {
    const raw = this.getPayload()?.DepartmentId;
    return raw ? parseInt(raw, 10) : null;
  }

  getFullName(): string {
    return this.getPayload()?.name ?? '';
  }

  getEmail(): string {
    return this.getPayload()?.email ?? '';
  }

  getUserId(): string {
    return this.getPayload()?.sub ?? '';
  }

  isTokenExpired(): boolean {
    const payload = this.getPayload();
    if (!payload) return true;
    return payload.exp * 1000 < Date.now();
  }

  private setTokens(res: AuthResponse) {
    localStorage.setItem(this.TOKEN_KEY, res.accessToken);
    localStorage.setItem(this.REFRESH_KEY, res.refreshToken);
    this.isLoggedIn.set(true);
  }
}

import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

interface TokenPayload {
  sub: string;
  email: string;
  name: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  exp: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private readonly API = environment.apiBaseUrl;
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_KEY = 'refresh_token';

  isLoggedIn = signal(!!localStorage.getItem(this.TOKEN_KEY));

  login(email: string, password: string) {
    return this.http.post<AuthResponse>(`${this.API}/auth/login`, { email, password }).pipe(
      tap(res => this.setTokens(res))
    );
  }

  register(email: string, password: string) {
    return this.http.post<AuthResponse>(`${this.API}/auth/register`, { email, password }).pipe(
      tap(res => this.setTokens(res))
    );
  }

  refresh() {
    const refreshToken = localStorage.getItem(this.REFRESH_KEY);
    if (!refreshToken) throw new Error('No refresh token');
    return this.http.post<AuthResponse>(`${this.API}/auth/refresh`, { refreshToken }).pipe(
      tap(res => this.setTokens(res))
    );
  }

  logout() {
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

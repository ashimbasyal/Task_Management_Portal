import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  template: `
    <div class="shell">
      <aside class="sidebar">
        <div class="sidebar-header">
          <h3>TMP</h3>
        </div>
        <nav class="sidebar-nav">
          <a routerLink="/dashboard" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }" class="nav-item">
            <i class="pi pi-home"></i>
            <span>Dashboard</span>
          </a>
          <a routerLink="/backlog" routerLinkActive="active" class="nav-item">
            <i class="pi pi-list"></i>
            <span>Backlog</span>
          </a>
          <a routerLink="/sprint" routerLinkActive="active" class="nav-item">
            <i class="pi pi-flag"></i>
            <span>Sprint</span>
          </a>
          <a *ngIf="role === 'Admin'" routerLink="/master-data" routerLinkActive="active" class="nav-item">
            <i class="pi pi-cog"></i>
            <span>Master Data</span>
          </a>
          <a *ngIf="role === 'Admin'" routerLink="/admin/users" routerLinkActive="active" class="nav-item">
            <i class="pi pi-users"></i>
            <span>Admin Panel</span>
          </a>
        </nav>
        <div class="sidebar-footer">
          <button class="logout-btn" (click)="logout()">
            <i class="pi pi-sign-out"></i>
            Logout
          </button>
        </div>
      </aside>
      <main class="main-content">
        <header class="topbar">
          <div></div>
          <div class="user-info">
            <div class="user-avatar">
              <img [src]="avatarUrl" (error)="avatarError.set(true)" *ngIf="!avatarError()" alt="Avatar" />
              <span *ngIf="avatarError()">{{ initials }}</span>
            </div>
            <div class="user-details">
              <span class="user-name">{{ fullName }}</span>
              <span class="user-email">{{ email }}</span>
            </div>
          </div>
        </header>
        <div class="content-body">
          <router-outlet />
        </div>
      </main>
    </div>
  `,
  styles: [`
    .shell { display: flex; min-height: 100vh; background: #f8f8f8; }
    .sidebar {
      width: 240px; background: #fff; border-right: 1px solid #e2e8f0;
      display: flex; flex-direction: column; padding: 1rem 0;
      position: fixed; top: 0; left: 0; bottom: 0;
    }
    .sidebar-header {
      padding: 0 1.25rem 1rem; border-bottom: 1px solid #e2e8f0; margin-bottom: 0.5rem;
    }
    .sidebar-header h3 { margin: 0; font-size: 1.25rem; font-weight: 700; color: #2563eb; }
    .sidebar-nav { flex: 1; display: flex; flex-direction: column; gap: 2px; padding: 0 0.5rem; }
    .nav-item {
      display: flex; align-items: center; gap: 0.6rem;
      padding: 0.65rem 0.75rem; border-radius: 8px;
      color: #475569; text-decoration: none; font-size: 0.9rem; font-weight: 500;
      transition: all 0.15s;
    }
    .nav-item:hover { background: #f1f5f9; color: #1e293b; }
    .nav-item.active { background: #eff6ff; color: #2563eb; font-weight: 600; }
    .nav-item i { font-size: 1.1rem; width: 20px; text-align: center; }
    .sidebar-footer {
      padding: 1rem 1.25rem 0; border-top: 1px solid #e2e8f0;
      margin-top: 0.5rem;
    }
    .logout-btn {
      display: flex; align-items: center; gap: 0.5rem; width: 100%;
      padding: 0.5rem 0.75rem; border: none; border-radius: 8px;
      background: #fef2f2; color: #ef4444; font-size: 0.85rem; font-weight: 500;
      cursor: pointer; transition: background 0.15s;
    }
    .logout-btn:hover { background: #fee2e2; }
    .main-content { flex: 1; margin-left: 240px; display: flex; flex-direction: column; }
    .topbar {
      display: flex; align-items: center; justify-content: space-between;
      padding: 0.75rem 2rem; background: #fff;
      border-bottom: 1px solid #e2e8f0;
    }
    .user-info { display: flex; align-items: center; gap: 0.75rem; }
    .user-avatar {
      width: 36px; height: 36px; border-radius: 50%;
      background: #2563eb; color: #fff; overflow: hidden;
      display: flex; align-items: center; justify-content: center;
      font-size: 0.85rem; font-weight: 700; flex-shrink: 0;
    }
    .user-avatar img { width: 100%; height: 100%; object-fit: cover; }
    .user-details { display: flex; flex-direction: column; line-height: 1.3; }
    .user-name { font-size: 0.9rem; font-weight: 600; color: #1e293b; }
    .user-email { font-size: 0.75rem; color: #94a3b8; }
    .content-body { flex: 1; padding: 2rem; }
  `]
})
export class LayoutComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  role = this.auth.getRole();
  fullName = this.auth.getFullName();
  email = this.auth.getEmail();
  avatarUrl = 'https://cdn.arthakendra.com/sharehub/icons/2024/08/13/091126-heip.png';
  avatarError = signal(false);

  get initials(): string {
    return this.fullName
      .split(' ')
      .map(w => w[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }

  logout() {
    this.auth.logout();
  }
}

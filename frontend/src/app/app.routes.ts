import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/auth/auth.guard';
import { permissionGuard } from './core/guards/permission.guard';
import { Permission } from './core/auth/permission.enum';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/login/login.component').then(c => c.LoginComponent),
    canActivate: [guestGuard],
  },
  {
    path: 'register',
    loadComponent: () => import('./features/register/register.component').then(c => c.RegisterComponent),
    canActivate: [guestGuard],
  },
  {
    path: '',
    loadComponent: () => import('./shared/layout/layout.component').then(c => c.LayoutComponent),
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        canActivate: [permissionGuard([Permission.ViewDashboard])],
        loadComponent: () => import('./features/dashboard/dashboard.component').then(c => c.DashboardComponent),
      },
      {
        path: 'backlog',
        canActivate: [permissionGuard([Permission.ViewBacklog])],
        loadComponent: () => import('./features/backlog/backlog.component').then(c => c.BacklogComponent),
      },
      {
        path: 'sprint',
        canActivate: [permissionGuard([Permission.ViewSprint])],
        loadComponent: () => import('./features/sprint/sprint.component').then(c => c.SprintComponent),
      },
      {
        path: 'master-data',
        canActivate: [permissionGuard([Permission.ViewMasterData])],
        loadComponent: () => import('./features/master-data/master-data.component').then(c => c.MasterDataComponent),
      },
      {
        path: 'admin',
        canActivate: [permissionGuard([Permission.ViewUsers])],
        children: [
          {
            path: 'user-role',
            loadComponent: () => import('./features/admin/users/user-list.component').then(c => c.UserListComponent),
          },
          {
            path: 'audit-log',
            canActivate: [permissionGuard([Permission.ViewAuditLogs])],
            loadComponent: () => import('./features/admin/audit-log/audit-log.component').then(c => c.AuditLogComponent),
          },
          { path: '', redirectTo: 'user-role', pathMatch: 'full' },
        ],
      },
      { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: '/login' },
];

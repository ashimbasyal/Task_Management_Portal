import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/guards/role.guard';

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
        loadComponent: () => import('./features/dashboard/dashboard.component').then(c => c.DashboardComponent),
      },
      {
        path: 'backlog',
        loadComponent: () => import('./features/backlog/backlog.component').then(c => c.BacklogComponent),
      },
      {
        path: 'sprint',
        loadComponent: () => import('./features/sprint/sprint.component').then(c => c.SprintComponent),
      },
      {
        path: 'master-data',
        canActivate: [roleGuard(['Admin'])],
        loadComponent: () => import('./features/master-data/master-data.component').then(c => c.MasterDataComponent),
      },
      {
        path: 'admin',
        canActivate: [roleGuard(['Admin'])],
        children: [
          {
            path: 'users',
            loadComponent: () => import('./features/admin/users/user-list.component').then(c => c.UserListComponent),
          },
          { path: '', redirectTo: 'users', pathMatch: 'full' },
        ],
      },
      { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: '/login' },
];

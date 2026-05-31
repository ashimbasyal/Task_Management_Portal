import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { Permission } from '../auth/permission.enum';

export function permissionGuard(requiredPermissions: Permission[]): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const userPermissions = auth.getPermissions();
    const hasAll = requiredPermissions.every(p => userPermissions.includes(p));
    if (hasAll) return true;
    router.navigate(['/dashboard']);
    return false;
  };
}

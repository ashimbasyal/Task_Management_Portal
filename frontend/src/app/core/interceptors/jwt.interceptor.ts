import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { catchError, from, switchMap, throwError } from 'rxjs';

const skipRefreshPaths = ['/auth/login', '/auth/register', '/auth/refresh'];

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();
  if (token) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }
  return next(req).pipe(
    catchError(err => {
      if (err instanceof HttpErrorResponse && err.status === 401 && token
        && !skipRefreshPaths.some(p => req.url.includes(p))) {
        return from(auth.refresh()).pipe(
          switchMap(() => {
            const freshToken = auth.getToken();
            const cloned = req.clone({ setHeaders: { Authorization: `Bearer ${freshToken}` } });
            return next(cloned);
          }),
          catchError(refreshErr => throwError(() => refreshErr))
        );
      }
      return throwError(() => err);
    })
  );
};

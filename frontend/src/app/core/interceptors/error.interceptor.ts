import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MessageService } from 'primeng/api';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);
  return next(req).pipe(
    catchError(err => {
      const errors = err.error?.errors;
      const detail = Array.isArray(errors)
        ? errors.join(', ')
        : err.error?.title || err.error?.message || err.message || 'An error occurred';
      messageService.add({ severity: 'error', summary: 'Error', detail, key: 'br' });
      return throwError(() => err);
    })
  );
};

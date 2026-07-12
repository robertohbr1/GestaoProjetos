import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError(err => {
      if ([401, 403].includes(err.status)) {
        authService.logout();
        router.navigate(['/login']);
      }
      const error = err.error?.message || err.statusText;
      return throwError(() => error);
    })
  );
};

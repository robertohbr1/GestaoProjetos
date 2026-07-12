import { Injectable, inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    // Check role restriction if specified in route data
    const expectedRoles = route.data['roles'] as Array<string>;
    if (expectedRoles) {
      const userRole = authService.userRole();
      if (!userRole || !expectedRoles.includes(userRole)) {
        router.navigate(['/']);
        return false;
      }
    }
    return true;
  }

  router.navigate(['/login']);
  return false;
};

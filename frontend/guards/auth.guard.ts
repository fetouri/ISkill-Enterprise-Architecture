import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformServer } from '@angular/common';
import { CanActivateFn, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthServiceService } from '../services/auth-service.service';
import { jwtDecode } from 'jwt-decode';

export const authGuard: CanActivateFn = (route, state) => {
  const platformId = inject(PLATFORM_ID);

  // During SSR (Server Side Rendering), browser storage (localStorage & document.cookie)
  // is inaccessible on the Node.js server. Allowing pass-through on the server prevents SSR
  // from falsely rendering and serving the /login page on page refresh.
  // The actual authentication check executes securely on client hydration in the browser.
  if (isPlatformServer(platformId)) {
    return true;
  }

  const cookieService = inject(CookieService);
  const authService = inject(AuthServiceService);
  const router = inject(Router);
  const user = authService.getUser();

  // Check for the JWT Token
  let token = cookieService.get('Authorization');

  if (token && user) {
    token = token.replace('Bearer ', '');
    try {
      const decodedToken: any = jwtDecode(token);
      const expirationDate = decodedToken.exp * 1000;
      const currentTime = new Date().getTime();

      if (expirationDate < currentTime) {
        authService.logout();
        return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
      }
    } catch (e) {
      // Allow fallback if user object is present in session
    }

    // Check if target is an Admin-only route
    const targetUrl = state.url || '';
    if (targetUrl.startsWith('/admin')) {
      if (user.roles && user.roles.includes('Writer')) {
        return true;
      } else {
        // Normal user trying to access admin: redirect gracefully to user profile
        return router.createUrlTree(['/user/profile']);
      }
    }

    // Normal authenticated user routes (like /user/profile, /user/serviceAppList, /user/add-ServiceApp)
    return true;
  } else {
    // Not logged in
    authService.logout();
    return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
  }
};

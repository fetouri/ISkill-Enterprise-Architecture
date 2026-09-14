import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginRequest } from '../models/login-request-model';
import { LoginResponse } from '../models/Login-response-model';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CookieService } from 'ngx-cookie-service';
import { User } from '../models/user-model';
import { RegisterRequest } from '../models/register-request-modle';

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {

  $user = new BehaviorSubject<User | undefined>(undefined);

  constructor(private http: HttpClient, private cookieService: CookieService) {
    const initialUser = this.getUser();
    if (initialUser) {
      this.$user.next(initialUser);
    }
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiBaseUrl}/api/Auth/login`, {
      email: request.email,
      password: request.password
    }).pipe(
      tap(response => {
        if (typeof window !== 'undefined') {
          localStorage.setItem('user-email', response.email);
          localStorage.setItem('user-roles', response.roles.join(','));
          localStorage.setItem('user-id', response.id);
        }
        this.$user.next({
          email: response.email,
          roles: response.roles,
          id: response.id
        });
      })
    );
  }

  register(request: RegisterRequest): Observable<void> {
    return this.http.post<void>(`${environment.apiBaseUrl}/api/Auth/register`, request);
  }

  setUser(user: User): void {
    this.$user.next(user);
    if (typeof window !== 'undefined') {
      localStorage.setItem('user-email', user.email);
      localStorage.setItem('user-roles', user.roles.join(','));
      if (user.id) {
        localStorage.setItem('user-id', user.id);
      }
    }
  }

  user(): Observable<User | undefined> {
    return this.$user.asObservable();
  }

  getUser(): User | undefined {
    if (typeof window === 'undefined') return undefined;
    const email = localStorage.getItem('user-email');
    const roles = localStorage.getItem('user-roles');
    const userId = localStorage.getItem('user-id') || 'usr-' + (email ? email.split('@')[0] : '1');

    if (email) {
      const user: User = {
        email: email,
        roles: roles ? roles.split(',') : ['User'],
        id: userId
      };
      return user;
    }
    return undefined;
  }

  logout(): void {
    if (typeof window !== 'undefined') {
      localStorage.clear();
    }
    this.cookieService.delete('Authorization', '/');
    this.$user.next(undefined);
  }

  isLoggedIn(): boolean {
    return !!this.cookieService.get('Authorization');
  }

  changePassword(request: { email: string; currentPassword: string; newPassword: string }): Observable<any> {
    return this.http.post(`${environment.apiBaseUrl}/api/Auth/change-password`, request);
  }
}

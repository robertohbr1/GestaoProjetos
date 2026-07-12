import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { LoginResponse } from '../../shared/models/models';

interface DecodedToken {
  nameid?: string;
  unique_name?: string;
  role?: string;
  exp?: number;
  [key: string]: any;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5151/api/auth';
  private tokenKey = 'gp_auth_token';

  // Signals
  token = signal<string | null>(null);
  currentUser = signal<string | null>(null);
  userRole = signal<string | null>(null);
  userId = signal<number | null>(null);
  
  isLoggedIn = computed(() => !!this.token());

  constructor(private http: HttpClient) {
    this.loadToken();
  }

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, { username, password }).pipe(
      tap(res => {
        this.saveToken(res.token);
      })
    );
  }

  register(username: string, email: string, password: string, role: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, { username, email, password, role });
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.token.set(null);
    this.currentUser.set(null);
    this.userRole.set(null);
    this.userId.set(null);
  }

  private saveToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
    this.token.set(token);
    this.decodeAndSetUser(token);
  }

  private loadToken(): void {
    const token = localStorage.getItem(this.tokenKey);
    if (token) {
      if (this.isTokenExpired(token)) {
        this.logout();
      } else {
        this.token.set(token);
        this.decodeAndSetUser(token);
      }
    }
  }

  private decodeAndSetUser(token: string): void {
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      const name = decoded.unique_name || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || null;
      const role = decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
      const idStr = decoded.nameid || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || null;

      this.currentUser.set(name);
      this.userRole.set(role);
      this.userId.set(idStr ? parseInt(idStr, 10) : null);
    } catch {
      this.logout();
    }
  }

  private isTokenExpired(token: string): boolean {
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      if (!decoded.exp) return false;
      return (Math.floor(new Date().getTime() / 1000)) >= decoded.exp;
    } catch {
      return true;
    }
  }
}

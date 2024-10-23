import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Token } from '@angular/compiler';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://localhost:7250/api/User';
  private refreshTokenKey = 'refreshToken';
  private jwtTokenKey = 'jwtToken';
  private userIdKey = 'UserId';
  USER_ID: any = '';

  constructor(private http: HttpClient, private router: Router) {}

  // Get JWT token from storage
  getJwtToken() {
    return sessionStorage.getItem(this.jwtTokenKey);
  }

  // Get refresh token from storage
  getRefreshToken() {
    return sessionStorage.getItem(this.refreshTokenKey);
  }

  // Set JWT token in session storage
  setJwtToken(token: string) {
    sessionStorage.setItem(this.jwtTokenKey, token);
  }

  setUserId(data: string){
    sessionStorage.setItem(this.userIdKey, data)
  }

  getUserId(){
    return sessionStorage.getItem(this.userIdKey);
  }

  // Set refresh token in session storage
  setRefreshToken(token: string) {
    sessionStorage.setItem(this.refreshTokenKey, token);
  }

  // Remove tokens on logout
  clearTokens() {
    sessionStorage.removeItem(this.jwtTokenKey);
    sessionStorage.removeItem(this.refreshTokenKey);
  }

  // Login function
  login(username: string, password: string): Observable<any> {
    const loginObj = { username, password };
    return this.http.post(`${this.apiUrl}/login`, loginObj).pipe(
      tap((response: any) => {
        if (response && response.success) {
          this.setJwtToken(response.data.jwtToken); // Assuming new JWT is in `response.data.JwtToken`
          this.setRefreshToken(response.data.RefreshToken); // Assuming refresh token is in `response.data.RefreshToken`
          this.setUserId(response.userId);
        }
      }),
      catchError(error => {
        console.error('Login failed', error);
        return of(null); // Handle error gracefully
      })
    );
  }

  // Refresh token function
  refreshToken(): Observable<any> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      this.logout(); // No refresh token, log out user
      return of(null);
    }

    return this.http.post(`${this.apiUrl}/refresh-token`, { refreshToken }).pipe(
      tap((response: any) => {
        if (response && response.success) {
          this.setJwtToken(response.data.JwtToken); // Update JWT token
          this.setRefreshToken(response.data.RefreshToken); // Update refresh token
        } else {
          this.logout(); // Token refresh failed, log out user
        }
      }),
      catchError(error => {
        console.error('Error refreshing token', error);
        this.logout(); // Log out in case of any error
        return of(null);
      })
    );
  }

  // Check if the token is expired or about to expire
  isTokenExpired(token: string): boolean {
    const expiry = JSON.parse(atob(token.split('.')[1])).exp;
    const now = Math.floor(new Date().getTime() / 1000);
    return now >= expiry;
  }

  // Automatically refresh token if it's about to expire
  autoRefreshToken(): Observable<boolean> {
    const token = this.getJwtToken();
    if (!token || this.isTokenExpired(token)) {
      // If the token is expired or missing, try to refresh
      return this.refreshToken().pipe(
        map(() => true),
        catchError(() => {
          this.logout();
          return of(false);
        })
      );
    }

    return of(true); // Token is valid
  }

  // Logout function
  logout() {
    this.clearTokens();
    this.router.navigateByUrl('/login');
  }

  // Check if the user is logged in
  isLoggedIn(): boolean {
    return !!this.getJwtToken(); // Check if JWT token exists
  }
}

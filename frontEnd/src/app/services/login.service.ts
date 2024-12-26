import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Login } from '../model/login';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  baseUrl: string = 'http://localhost:5193/api'; // Update if backend port changes

  constructor(private http: HttpClient) {}

  // Store Authorization header
  storeAuthHeader(email: string, passwordHash: string): void {
    const authHeader = `Basic ${btoa(`${email}:${passwordHash}`)}`;
    localStorage.setItem('authHeader', authHeader); // Use sessionStorage if needed
  }

  // Get all logins
  Logins(): Observable<Login[]> {
    return this.http
      .get<Login[]>(`${this.baseUrl}/Login`) // Assuming /login returns a list of logins
      .pipe(catchError(this.handleError)); // Error handling
  }

  // Login user
  Login(credentials: Login): Observable<any> {
    return this.http
      .post<any>(`${this.baseUrl}/Login/login`, credentials)
      .pipe(
        catchError(this.handleError),
        // Store the Authorization header on successful login
        tap((response: any) => {
          this.storeAuthHeader(credentials.email, credentials.passwordHash);
        })
      );
  }

  // Retrieve Authorization header
  getAuthHeader(): string | null {
    const authHeader = localStorage.getItem('authHeader');
    if (!authHeader) {
      console.warn('Authorization header is missing.');
    }
    return authHeader;
  }

  // Error handling method
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client-side error: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.status === 401) {
        errorMessage = 'Unauthorized: Please check your credentials.';
      } else if (error.status === 500) {
        errorMessage = 'Server error: Please try again later.';
      } else {
        errorMessage = `Server-side error: Error Code: ${error.status}\nMessage: ${error.message}`;
      }
    }
    console.error('LoginService Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Login } from '../model/login';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  baseUrl: string = 'http://localhost:5193/api'; // Update if backend port changes

  constructor(private http: HttpClient) {}

  // Get all logins
  Logins(): Observable<Login[]> {
    return this.http
      .get<Login[]>(`${this.baseUrl}/login`) // Assuming /login returns a list of logins
      .pipe(catchError(this.handleError)); // Error handling
  }

  // Login user
  Login(credentials: Login): Observable<any> {
    return this.http
      .post<any>(`${this.baseUrl}/login/login`, credentials)
      .pipe(catchError(this.handleError)); // Error handling
  }

  // Error handling method
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client-side error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Server-side error: Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    console.error('LoginService Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
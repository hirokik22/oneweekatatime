import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class SignUpService {
  private baseUrl = 'http://localhost:5193/api/'; 

  constructor(private http: HttpClient) {}

  /**
   * Sign-up method to create a login and associated roomies
   * @param signUpData - Contains email, password, and an array of roomie names
   */
  signUp(signUpData: { email: string; passwordHash: string; roomieNames: string[] }): Observable<any> {
    const url = `${this.baseUrl}Login/sign-up`; 

    // Generate the Basic Auth header
    const authHeader = this.createAuthHeader(signUpData.email, signUpData.passwordHash);

    return this.http
      .post(url, signUpData, {
        headers: { Authorization: authHeader }, // Include Authorization header
      })
      .pipe(
        map((response: any) => {
          if (response?.success) {
            return response; // In case of Success 
          } else {
            throw new Error(response?.message || 'Unexpected error occurred.');
          }
        }),
        catchError(this.handleError) 
      );
  }

  /**
   *  Basic Auth header
   * @param email - User email
   * @param passwordHash - User password 
   */
  private createAuthHeader(email: string, passwordHash: string): string {
    return `Basic ${btoa(`${email}:${passwordHash}`)}`;
  }

  /**
   * Error handling for HTTP requests
   * @param error - HttpErrorResponse
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client-side error: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.status === 401) {
        errorMessage = 'Unauthorized: Invalid email or password.';
      } else if (error.status === 400) {
        errorMessage = 'Bad request: Please check your input.';
      } else {
        errorMessage = `Server-side error: Code ${error.status}\nMessage: ${error.message}`;
      }
    }
    console.error('SignUpService Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
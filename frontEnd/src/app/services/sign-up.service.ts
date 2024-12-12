import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class SignUpService {
  private baseUrl = 'http://localhost:5193/api'; // Replace with your actual backend URL

  constructor(private http: HttpClient) {}

  /**
   * Sign-up method to create a login and associated roomies
   * @param signUpData - Contains email, password, and an array of roomie names
   */
  signUp(signUpData: { email: string; passwordHash: string; roomieNames: string[] }): Observable<any> {
    const url = `${this.baseUrl}/login/sign-up`; // Ensure this matches your backend endpoint
    return this.http.post(url, signUpData).pipe(
      catchError(this.handleError) // Handle HTTP errors
    );
  }

  /**
   * Error handling for HTTP requests
   * @param error - HttpErrorResponse
   */
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client-side error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Server-side error: Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    console.error('SignUpService Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
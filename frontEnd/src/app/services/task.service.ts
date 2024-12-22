import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Task } from '../model/task';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  baseUrl: string = 'http://localhost:5193/api'; // Update if backend port changes

  constructor(private http: HttpClient) {}

  // Get all tasks
  getTasks(): Observable<Task[]> {
    return this.http
      .get<Task[]>(`${this.baseUrl}/task`)
      .pipe(catchError(this.handleError));
  }

  // Get tasks by Login ID
  getTasksByLoginId(loginId: number): Observable<Task[]> {
    return this.http
      .get<Task[]>(`${this.baseUrl}/Task?loginId=${loginId}`)
      .pipe(catchError(this.handleError));
  }
  

  // Get a single task by ID
  getTask(taskId: number): Observable<Task> {
    return this.http
      .get<Task>(`${this.baseUrl}/task/${taskId}`)
      .pipe(catchError(this.handleError));
  }

  // Create a new task
  createTask(task: Partial<Task>): Observable<any> {
    return this.http
      .post(`${this.baseUrl}/task`, task)
      .pipe(catchError(this.handleError));
  }

  // Update an existing task
  updateTask(task: Task): Observable<any> {
    return this.http
      .put(`${this.baseUrl}/task/${task.taskId}`, task)
      .pipe(catchError(this.handleError));
  }

  // Delete a task by ID
  deleteTask(taskId: number): Observable<any> {
    return this.http
      .delete(`${this.baseUrl}/task/${taskId}`)
      .pipe(catchError(this.handleError));
  }

    // **NEW METHOD**: Get all roomies
    getRoomies(): Observable<{ roomieId: number; roomieName: string }[]> {
      return this.http
        .get<{ roomieId: number; roomieName: string }[]>(`${this.baseUrl}/Roomie`)
        .pipe(catchError(this.handleError));
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
    console.error('TaskService Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}

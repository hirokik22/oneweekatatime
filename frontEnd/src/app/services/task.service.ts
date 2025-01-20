import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Task } from '../model/task';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  baseUrl: string = 'http://localhost:5193/api'; 

  constructor(private http: HttpClient) {}

  /**
   * Retrieve the Authorization header from localStorage
   */
  private getAuthHeader(): HttpHeaders {
    const authHeader = localStorage.getItem('authHeader');
    if (!authHeader) {
      console.error('Authorization header is missing.');
      throw new Error('User is not authenticated. Please log in again.');
    }
    return new HttpHeaders({ Authorization: authHeader });
  }

  // Get all tasks
  getTasks(): Observable<Task[]> {
    return this.http
      .get<Task[]>(`${this.baseUrl}/task`, { headers: this.getAuthHeader() })
      .pipe(catchError(this.handleError));
  }

  // Get tasks by Login ID
  getTasksByLoginId(loginId: number): Observable<Task[]> {
    return this.http
      .get<Task[]>(`${this.baseUrl}/Task?loginId=${loginId}`, { headers: this.getAuthHeader() })
      .pipe(catchError(this.handleError));
  }

  // Get a single task by ID
  getTask(taskId: number): Observable<Task> {
    return this.http
      .get<Task>(`${this.baseUrl}/task/${taskId}`, { headers: this.getAuthHeader() })
      .pipe(catchError(this.handleError));
  }

  // Create a new task
  createTask(task: Partial<Task>, roomieIds: number[]): Observable<{ message: string; task: Task }> {
    const payload = { ...task, roomieIds }; // Combine task and roomieIds into a single payload
    return this.http
      .post<{ message: string; task: Task }>(`${this.baseUrl}/task`, payload, { headers: this.getAuthHeader() })
      .pipe(
        catchError((error: HttpErrorResponse) => {
          console.error('Error occurred during task creation:', error);
          return throwError(() => new Error('Failed to create task.'));
        })
      );
  }
  

  // Update an existing task /*FRANCESCO414

  updateTask(task: Task, roomieIds: number[]): Observable<any> {
    if (!task.taskId) {
      throw new Error('Task ID is missing in the request.');
    }
  
    // Costruisci il payload per includere sempre i roomieIds
    const payload = {
      taskId: task.taskId,
      taskName: task.taskName,
      note: task.note,
      isCompleted: task.isCompleted, // Aggiorna lo stato
      dayOfWeek: task.dayOfWeek,
      taskOrder: task.taskOrder,
      loginId: task.loginId,
      roomieIds: roomieIds // Roomie assegnati inclusi
    };
  
    return this.http
      .put(`${this.baseUrl}/task/${task.taskId}`, payload, { headers: this.getAuthHeader() })
      .pipe(
        catchError((error: HttpErrorResponse) => {
          console.error(`Error updating task with ID ${task.taskId}:`, error.message);
          return throwError(() => new Error(`Failed to update task: ${error.message}`));
        })
      );
  }
  
  



  // Delete a task by ID
  deleteTask(taskId: number): Observable<any> {
    return this.http
      .delete(`${this.baseUrl}/task/${taskId}`, { headers: this.getAuthHeader() })
      .pipe(catchError(this.handleError));
  }

  // Get all roomies
  getRoomies(): Observable<{ roomieId: number; roomieName: string }[]> {
    return this.http
      .get<{ roomieId: number; roomieName: string }[]>(`${this.baseUrl}/Roomie`, { headers: this.getAuthHeader() })
      .pipe(catchError(this.handleError));
  }

  getRoomiesByLoginId(loginId: number): Observable<{ roomieId: number; roomieName: string }[]> {
    return this.http
      .get<{ roomieId: number; roomieName: string }[]>(`${this.baseUrl}/Roomie/${loginId}`)
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
      if (error.status === 401) {
        errorMessage = 'Unauthorized: Please check your credentials.';
      } else if (error.status === 403) {
        errorMessage = 'Forbidden: You do not have permission to perform this action.';
      } else {
        errorMessage = `Server-side error: Error Code: ${error.status}\nMessage: ${error.message}`;
      }
    }
    console.error('TaskService Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
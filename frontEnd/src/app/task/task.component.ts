import { Component } from '@angular/core';
import { TaskService } from '../services/task.service';
import { RoomieService } from '../services/roomie.service';
import { Task } from '../model/task';
import { Roomie } from '../model/roomie';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common'; // Import CommonModule

@Component({
  selector: 'app-task',
  standalone: true,
  imports: [CommonModule, FormsModule], // Add CommonModule here
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css'],
})
export class TaskComponent {
  errorMessage: string | null = null;
  tasks: Task[] = [];
  roomies: Roomie[] = []; // Array to hold roomies
  daysOfWeek: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  showTaskForm: { [day: string]: boolean } = {}; // Tracks visibility of the form for each day

  newTask: Task = {
    taskId: 0,
    taskName: '',
    assignedRoomie: null,
    note: '',
    dayOfWeek: '',
    isCompleted: false,
    taskOrder: 0,
    LoginID: 0, // Updated field for LoginId
  };

  constructor(private taskService: TaskService, private roomieService: RoomieService) {}

  ngOnInit(): void {
    this.loadTasks();
    this.loadRoomies();
    this.daysOfWeek.forEach((day) => (this.showTaskForm[day] = false));
  }

  getDaysOfWeek(): string[] {
    return this.daysOfWeek;
  }

  getTasksForDay(day: string): Task[] {
    return this.tasks.filter((task) => task.dayOfWeek === day);
  }

  toggleTaskForm(day: string): void {
    this.showTaskForm[day] = !this.showTaskForm[day];
    if (this.showTaskForm[day]) {
      const loginId = this.getLoginIdFromStorage();
      this.newTask = {
        taskId: 0,
        taskName: '',
        assignedRoomie: null,
        note: '',
        dayOfWeek: day,
        isCompleted: false,
        taskOrder: this.getTasksForDay(day).length + 1,
        LoginID: loginId || 0, // Assign LoginId to the new task
      };
    }
  }

  addTaskForDay(): void {
    if (!this.newTask.taskName) {
      this.errorMessage = 'Task Name is required.';
      return;
    }

    this.taskService.createTask(this.newTask).subscribe({
      next: () => {
        this.errorMessage = null;
        this.loadTasks();
        this.showTaskForm[this.newTask.dayOfWeek] = false; // Hide the form after task creation
      },
      error: (err) => {
        this.errorMessage = err.message || 'Failed to create task.';
      },
    });
  }

  deleteTask(taskId: number): void {
    this.taskService.deleteTask(taskId).subscribe({
      next: () => this.loadTasks(),
      error: (err) => {
        this.errorMessage = err.message || 'Failed to delete task.';
      },
    });
  }

  toggleTaskCompletion(task: Task): void {
    task.isCompleted = !task.isCompleted;

    // Exclude unnecessary fields
    const sanitizedTask = { ...task };
    delete (sanitizedTask as any).assignedRoomie;

    console.log('Updating task with sanitized payload:', sanitizedTask);

    this.taskService.updateTask(sanitizedTask).subscribe({
        next: () => this.loadTasks(),
        error: (err) => {
            this.errorMessage = `Failed to update task: ${err.message}`;
            console.error('Task update error:', err);
        },
    });
}

  private loadTasks(): void {
    const loginId = this.getLoginIdFromStorage(); // Retrieve the loginId
    console.log('Retrieved loginId:', loginId);

    if (loginId) {
      this.taskService.getTasksByLoginId(loginId).subscribe({
        next: (tasks) => {
          this.tasks = tasks;
        },
        error: (err) => {
          this.errorMessage = err.message || 'Failed to load tasks for this user.';
        },
      });
    } else {
      this.errorMessage = 'No user ID found. Please log in again.';
    }
  }

  private loadRoomies(): void {
    const loginId = this.getLoginIdFromStorage();
    if (loginId) {
      this.roomieService.getRoomiesByLoginId(loginId).subscribe({
        next: (roomies) => {
          console.log('Fetched roomies:', roomies);
          this.roomies = roomies;
        },
        error: (err) => {
          this.errorMessage = err.message || 'Failed to load roomies for this user.';
        },
      });      
    }
  }

  // Helper method to retrieve loginId from session storage or other source
  private getLoginIdFromStorage(): number | null {
    console.log('Raw sessionStorage loginId:', sessionStorage.getItem('loginId'));
    const loginId = sessionStorage.getItem('loginId'); // Or replace with appropriate key
    return loginId ? Number(loginId) : null;
  }
}
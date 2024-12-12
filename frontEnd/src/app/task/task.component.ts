import { Component } from '@angular/core';
import { TaskService } from '../services/task.service';
import { Task } from '../model/task';
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
  daysOfWeek: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  showTaskForm: { [day: string]: boolean } = {}; // Tracks visibility of the form for each day

  // Initialize newTask with empty defaults
  newTask: Task = {
    taskId: 0,
    taskName: '',
    assignedRoomie: null, // Ensure type matches the model
    note: '',
    dayOfWeek: '',
    isCompleted: false,
    taskOrder: 0,
  };

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadTasks();
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
      this.newTask = {
        taskId: 0,
        taskName: '',
        assignedRoomie: null,
        note: '',
        dayOfWeek: day,
        isCompleted: false,
        taskOrder: this.getTasksForDay(day).length + 1,
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
    this.taskService.updateTask(task).subscribe({
      next: () => this.loadTasks(),
      error: (err) => {
        this.errorMessage = err.message || 'Failed to update task.';
      },
    });
  }

  private loadTasks(): void {
    this.taskService.getTasks().subscribe({
      next: (tasks) => {
        this.tasks = tasks;
      },
      error: (err) => {
        this.errorMessage = err.message || 'Failed to load tasks.';
      },
    });
  }
}

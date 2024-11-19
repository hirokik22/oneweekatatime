import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router'; // Import RouterOutlet for routing
import { PlannerComponent } from './planner/planner.component'; // Import PlannerComponent
import { TaskComponent } from './task/task.component'; // Import TaskComponent
import { RoomieComponent } from './roomie/roomie.component'; // Import RoomieComponent
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { LoginComponent } from './login/login.component';
import { SignUpComponent } from "./sign-up/sign-up.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    PlannerComponent,
    TaskComponent,
    RoomieComponent,
    CommonModule, // Add CommonModule for *ngFor and *ngIf
    ReactiveFormsModule,
    LoginComponent,
    SignUpComponent
],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'OneWeekAtATime';

  // Sample list of tasks
  tasks = [
    { id: 1, name: 'Do Laundry', dayOfWeek: 'Monday', isCompleted: false },
    { id: 1, name: 'Do Laundry', dayOfWeek: 'Monday', isCompleted: false },
    { id: 2, name: 'Buy Groceries', dayOfWeek: 'Tuesday', isCompleted: false },
    { id: 3, name: 'Clean Kitchen', dayOfWeek: 'Monday', isCompleted: true },
    { id: 4, name: 'Clean Kitchen', dayOfWeek: 'Friday', isCompleted: false }
  ];

  // Get the days of the week
  getDaysOfWeek(): string[] {
    return ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  }

  // Get tasks for a specific day
  getTasksForDay(day: string): any[] {
    return this.tasks.filter(task => task.dayOfWeek === day);
  }

  // Handle task updates from TaskComponent
  onTaskUpdated(updatedTask: { id: number; isCompleted: boolean }): void {
    const task = this.tasks.find(t => t.id === updatedTask.id);
    if (task) {
      task.isCompleted = updatedTask.isCompleted; // Update task status
      console.log('Task Updated:', task);
    }
  }
}

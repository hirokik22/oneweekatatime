import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Task } from './model/task'; // Import the Task model
import { TaskComponent } from './task/task.component'; // Import TaskComponent
import { LoginComponent } from './login/login.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [LoginComponent, RouterOutlet, TaskComponent], 
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'] // Correct property name
})
export class AppComponent {
  title = 'OneWeekAtATime';

  // Example list of tasks
  tasks: Task[] = [
    {
      taskID: 1,
      taskName: 'Do Laundry',
      assignedRoomie: 1, // Use RoomieID as per your database model
      note: 'Wash white clothes only',
      isCompleted: false,
      dayOfWeek: 'Monday',
      taskOrder: 1, // Assuming taskOrder is required
    },
    {
      taskID: 2,
      taskName: 'Buy Groceries',
      assignedRoomie: 2, // Use RoomieID as per your database model
      note: 'Milk, eggs, and bread',
      isCompleted: false,
      dayOfWeek: 'Tuesday',
      taskOrder: 2, // Assuming taskOrder is required
    },
  ];

  // Method to handle updates from TaskComponent
  onTaskUpdated(updatedTask: { id: number; isCompleted: boolean }): void {
    const task = this.tasks.find(t => t.taskID === updatedTask.id);
    if (task) {
      task.isCompleted = updatedTask.isCompleted; // Update the task's status
    }
  }
}

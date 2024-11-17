import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Task } from '../model/task'; // Adjust the path if necessary

@Component({
  selector: 'app-task',
  standalone: true,
  imports: [], // Add CommonModule if necessary
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css']
})
export class TaskComponent {
  // Input property to receive task data from the parent component
  @Input() task!: Task; // Ensure the Task interface is used here

  // Output property to emit task updates to the parent component
  @Output() taskUpdated = new EventEmitter<{ id: number; isCompleted: boolean }>();

  // Method to toggle the task's completion status
  toggleComplete(): void {
    this.task.isCompleted = !this.task.isCompleted; // Update the task's completion status]
    this.taskUpdated.emit({ id: this.task.taskID, isCompleted: this.task.isCompleted }); // Notify parent using taskID
  }
}

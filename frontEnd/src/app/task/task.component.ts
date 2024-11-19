import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Task } from '../model/task'; // Adjust the path as necessary

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
    this.task.isCompleted = !this.task.isCompleted; // Update the task's completion status
    this.taskUpdated.emit({ id: this.task.taskID, isCompleted: this.task.isCompleted }); // Notify parent using taskID
  }

  // Method to fetch the roomie name (if IDs are used instead of names)
    // Replace with logic to fetch the roomie's name (if it's not directly available in the task)
    getRoomieName(roomieId: number | string | null): string {
      if (roomieId === null) {
        return 'Unassigned'; // Handle unassigned roomie
      }
    
      const roomieList = [
        { id: 1, name: 'Alice' },
        { id: 2, name: 'Bob' },
        { id: 3, name: 'Charlie' },
      ];
    
      const roomie = roomieList.find(r => r.id === roomieId);
      return roomie ? roomie.name : 'Unknown'; // Return 'Unknown' if no match found
    }
  
  }


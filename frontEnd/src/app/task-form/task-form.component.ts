import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [FormsModule], // Import FormsModule and other modules if necessary
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.css']
})
export class TaskFormComponent {
  @Input() roomies: { id: number; name: string }[] = []; // Roomies input array
  @Output() taskSaved = new EventEmitter<any>(); // Event emitter to send the task back to parent

  task = {
    taskName: '',
    assignedRoomie: null,
    dayOfWeek: '',
    note: '',
  };

  getDaysOfWeek(): string[] {
    return ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  }

  onSubmit(): void {
    console.log('Task submitted:', this.task);
    this.taskSaved.emit(this.task); // Emit the task to the parent component
    this.resetForm();
  }

  resetForm(): void {
    this.task = {
      taskName: '',
      assignedRoomie: null,
      dayOfWeek: '',
      note: '',
    };
  }
}

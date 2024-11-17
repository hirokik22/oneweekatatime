import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-planner',
  standalone: true,
  imports: [CommonModule, FormsModule], // Import necessary modules
  templateUrl: './planner.component.html',
  styleUrls: ['./planner.component.css']
})
export class PlannerComponent {
  isFormVisible: boolean = false;

  taskForm = {
    taskName: '',
    note: '',
    dayOfWeek: '',
  };

  openTaskForm(day: string): void {
    this.isFormVisible = true;
    this.taskForm.dayOfWeek = day;
  }

  saveTask(): void {
    console.log('New Task:', this.taskForm);
    this.resetForm();
  }

  resetForm(): void {
    this.isFormVisible = false;
    this.taskForm = {
      taskName: '',
      note: '',
      dayOfWeek: '',
    };
  }
}

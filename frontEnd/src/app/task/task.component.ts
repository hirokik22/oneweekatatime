import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../services/task.service';
import { Task } from '../model/task';

@Component({
  selector: 'app-task',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css'],
  providers: [TaskService],
})
export class TaskComponent implements OnInit {
  tasks: Task[] = []; // Store all tasks

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.tasks = this.taskService.getTasks();
  }

  getDaysOfWeek(): string[] {
    return ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  }

  getTasksForDay(day: string): Task[] {
    return this.tasks.filter((task) => task.dayOfWeek === day);
  }

  addTaskForDay(day: string): void {
    const newTask: Task = {
      taskID: this.tasks.length + 1,
      taskName: 'New Task',
      assignedRoomie: null,
      note: 'Default note',
      isCompleted: false,
      dayOfWeek: day,
      taskOrder: this.tasks.length + 1,
    };

    this.tasks.push(newTask);
  }
  toggleTaskCompletion(task: Task): void {
    task.isCompleted = !task.isCompleted;
  }
  
}
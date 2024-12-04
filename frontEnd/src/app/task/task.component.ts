import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../services/task.service';
import { Task } from '../model/task';

@Component({
  selector: 'app-task',
  standalone: true,
  imports: [CommonModule], // Ensure CommonModule is included
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css'],
  providers: [TaskService], // Include TaskService for DI
})
export class TaskComponent implements OnInit {
  tasks: Task[] = [];
  errorMessage: string | null = null;

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe(
      (data) => {
        console.log('Tasks loaded:', data);
        this.tasks = data;
      },
      (error) => {
        this.errorMessage = 'Failed to load tasks. Please try again later.';
        console.error('Error fetching tasks:', error);
      }
    );
  }

  toggleTaskCompletion(task: Task): void {
    const updatedTask: Task = { ...task, isCompleted: !task.isCompleted };

    this.taskService.updateTask(updatedTask).subscribe(
      () => {
        const taskIndex = this.tasks.findIndex((t) => t.taskId === task.taskId);
        if (taskIndex !== -1) {
          this.tasks[taskIndex] = updatedTask;
        }
      },
      (error) => {
        this.errorMessage = 'Failed to update task completion. Please try again later.';
        console.error('Error updating task:', error);
      }
    );
  }

  addTaskForDay(day: string): void {
    const newTask: Task = {
      taskId: 0,
      taskName: `New Task for ${day}`,
      assignedRoomie: 0,
      note: '',
      isCompleted: false,
      dayOfWeek: day,
      taskOrder: this.tasks.filter((task) => task.dayOfWeek === day).length + 1,
    };

    this.taskService.createTask(newTask).subscribe(
      (createdTask) => {
        this.tasks.push(createdTask);
      },
      (error) => {
        this.errorMessage = 'Failed to add the task. Please try again later.';
        console.error('Error adding task:', error);
      }
    );
  }

  deleteTask(taskId: number): void {
    this.taskService.deleteTask(taskId).subscribe(
      () => {
        this.tasks = this.tasks.filter((task) => task.taskId !== taskId);
      },
      (error) => {
        this.errorMessage = 'Failed to delete the task. Please try again later.';
        console.error('Error deleting task:', error);
      }
    );
  }

  getDaysOfWeek(): string[] {
    return ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  }

  getTasksForDay(day: string): Task[] {
    return this.tasks.filter((task) => task.dayOfWeek === day);
  }
}
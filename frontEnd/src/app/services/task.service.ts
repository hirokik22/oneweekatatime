import { Injectable } from '@angular/core';
import { Task } from '../model/task';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private tasks: Task[] = [
    {
      taskID: 1,
      taskName: 'Do the laundry',
      assignedRoomie: 2,
      note: 'Separate whites and colors.',
      isCompleted: false,
      dayOfWeek: 'Monday',
      taskOrder: 1,
    },
    {
      taskID: 2,
      taskName: 'Grocery shopping',
      assignedRoomie: null,
      note: 'Buy milk, eggs, and bread.',
      isCompleted: false,
      dayOfWeek: 'Tuesday',
      taskOrder: 2,
    },
  ];

  getTasks(): Task[] {
    return this.tasks;
  }

  addTask(newTask: Task): void {
    this.tasks.push(newTask);
  }
}
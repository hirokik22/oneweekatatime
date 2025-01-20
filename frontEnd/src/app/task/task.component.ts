import { Component } from '@angular/core';
import { TaskService } from '../services/task.service';
import { RoomieService } from '../services/roomie.service';
import { Task } from '../model/task';
import { Roomie } from '../model/roomie';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common'; // Import CommonModule

@Component({
  selector: 'app-task',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css'],
})
export class TaskComponent {
  errorMessage: string | null = null;
  tasks: Task[] = [];
  roomies: Roomie[] = []; 
  daysOfWeek: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  showTaskForm: { [day: string]: boolean } = {}; 

  newTask: Task = {
    taskId: 0,
    taskName: '',
    note: '',
    dayOfWeek: '',
    isCompleted: false,
    taskOrder: 0,
    loginId: 0, 
    roomies: [], 
  };
  

  constructor(private taskService: TaskService, private roomieService: RoomieService) {}

  ngOnInit(): void {
    this.loadTasks();
    this.loadRoomies();
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
      const loginId = this.getLoginIdFromStorage();
      this.newTask = {
        taskId: 0,
        taskName: '',
        note: '',
        dayOfWeek: day,
        isCompleted: false,
        taskOrder: this.getTasksForDay(day).length + 1,
        loginId: loginId || 0, 
        roomies: [], 
      };
    }
  }  

  addTaskForDay(): void {
    if (!this.newTask.taskName) {
      this.errorMessage = 'Task Name is required.';
      return;
    }
  
    const roomieIds = this.newTask.roomies.map((roomie) => roomie.roomieid); 
  
    this.taskService.createTask(this.newTask, roomieIds).subscribe({
      next: () => {
        this.errorMessage = null;
        this.loadTasks();
        this.showTaskForm[this.newTask.dayOfWeek] = false;
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
    
   
      const roomieIds = task.roomies.map((roomie) => roomie.roomieid);
    
      // Ensure roomies are included in the sanitized task
      const sanitizedTask = {
        ...task,
        roomieIds: roomieIds // Include roomie IDs for backend processing
      };
    
      console.log('Updating task with sanitized payload:', sanitizedTask);
    
      // Send the update request
      this.taskService.updateTask(sanitizedTask, roomieIds).subscribe({
        next: () => {
          console.log(`Task ${task.taskId} updated successfully.`);
          this.loadTasks(); 
        },
        error: (err) => {
          this.errorMessage = `Failed to update task: ${err.message}`;
          console.error('Task update error:', err);
        },
      });
    }
    

    private loadTasks(): void {
      const loginId = this.getLoginIdFromStorage();
      if (loginId) {
        console.log('Login ID retrieved from storage:', loginId); // Debug login ID
    
        this.taskService.getTasksByLoginId(loginId).subscribe({
          next: (tasks) => {
            console.log('Tasks fetched from backend:', tasks); // Debug tasks fetched
    
            this.tasks = tasks.map((task) => {
              // Map assignedRoomies to Roomie objects
              const roomies = task.assignedRoomies?.map((name, index) => ({
                roomieid: index + 1, 
                loginid: loginId,   
                roomiename: name,    
              })) || [];
    
              console.log(`Task ID: ${task.taskId}, Roomies:`, roomies); // Debug each task and its roomies
    
              return {
                ...task,
                roomies, 
              };
            });
    
            console.log('Processed tasks with roomies:', this.tasks); 
          },
          error: (err) => {
            console.error('Error fetching tasks:', err); 
            this.errorMessage = err.message || 'Failed to load tasks for this user.';
          },
        });
      } else {
        console.warn('No user ID found in storage.'); 
        this.errorMessage = 'No user ID found. Please log in again.';
      }
    }  
  
  private loadRoomies(): void {
    const loginId = this.getLoginIdFromStorage();
    if (loginId) {
      this.roomieService.getRoomiesByLoginId(loginId).subscribe({
        next: (roomies) => {
          console.log('Fetched roomies:', roomies);
          this.roomies = roomies;
        },
        error: (err) => {
          this.errorMessage = err.message || 'Failed to load roomies for this user.';
        },
      });      
    }
  }

  // Helper method to retrieve loginId from session storage or other source
  private getLoginIdFromStorage(): number | null {
    console.log('Raw sessionStorage loginId:', sessionStorage.getItem('loginId'));
    const loginId = sessionStorage.getItem('loginId'); 
    return loginId ? Number(loginId) : null;
  }

  onRoomieSelectionChange(roomie: Roomie, event: Event): void {
    const checkbox = event.target as HTMLInputElement;
  
    // Ensure `newTask.roomies` is initialized
    if (!this.newTask.roomies) {
      this.newTask.roomies = [];
    }
  
    if (checkbox.checked) {
    
      if (!this.newTask.roomies.some(r => r.roomieid === roomie.roomieid)) {
        this.newTask.roomies.push(roomie);
      }
    } else {
      // Remove roomie from the array
      this.newTask.roomies = this.newTask.roomies.filter(
        r => r.roomieid !== roomie.roomieid
      );
    }
  }  

  getRoomieNames(task: Task): string {
    return task.roomies && task.roomies.length > 0
      ? task.roomies.map((r) => r.roomiename).join(', ')
      : 'Unassigned';
  }

  isRoomieAssigned(roomie: Roomie): boolean {
    return this.newTask.roomies.some(r => r.roomieid === roomie.roomieid);
  }

  getRoomiesForTask(task: any): string {
    if (!task.roomies || task.roomies.length === 0) {
      return 'Unassigned';
    }
    return task.roomies.map((roomie: any) => roomie.roomiename).join(', ');
  }   
}
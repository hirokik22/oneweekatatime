import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TaskComponent } from './task.component';
import { TaskService } from '../services/task.service';
import { RoomieService } from '../services/roomie.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { Task } from '../model/task';
import { Roomie } from '../model/roomie';

describe('TaskComponent', () => {
  let component: TaskComponent;
  let fixture: ComponentFixture<TaskComponent>;
  let taskService: jasmine.SpyObj<TaskService>;

  beforeEach(async () => {
    const taskServiceSpy = jasmine.createSpyObj('TaskService', [
      'getTasksByLoginId',
      'createTask',
      'updateTask',
      'deleteTask'
    ]);
  
    await TestBed.configureTestingModule({
      imports: [
        TaskComponent, // Import TaskComponent directly
        HttpClientTestingModule, // For HttpClient dependency
      ],
      providers: [
        { provide: TaskService, useValue: taskServiceSpy },
      ],
    }).compileComponents();
  
    fixture = TestBed.createComponent(TaskComponent);
    component = fixture.componentInstance;
    taskService = TestBed.inject(TaskService) as jasmine.SpyObj<TaskService>;
  
    // Mock sessionStorage to return a valid loginId
    spyOn(sessionStorage, 'getItem').and.returnValue('123');
  
    // Ensure the service methods return proper observables
    taskService.getTasksByLoginId.and.returnValue(of([])); // Return an empty tasks array
    taskService.updateTask.and.returnValue(of({}));        
    taskService.deleteTask.and.returnValue(of({}));       
  });
  

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load tasks on initialization', () => {
    const mockTasks: Task[] = [
      {
        taskId: 1,
        taskName: 'Test Task 1',
        note: 'Sample note',
        dayOfWeek: 'Monday',
        isCompleted: false,
        taskOrder: 1,
        loginId: 123,
        roomies: [],
      },
    ];

    // Mock the service method
    taskService.getTasksByLoginId.and.returnValue(of(mockTasks));


    component.ngOnInit();
    fixture.detectChanges();

    expect(taskService.getTasksByLoginId).toHaveBeenCalledWith(123);


    expect(component.tasks).toEqual(mockTasks);
  });

  it('should handle error when loading tasks', () => {
    const error = { message: 'Failed to load tasks.' };

    // Mock the service method to throw an error
    taskService.getTasksByLoginId.and.returnValue(throwError(() => error));

    // Trigger ngOnInit and change detection
    component.ngOnInit();
    fixture.detectChanges();

    // Verify service method was called
    expect(taskService.getTasksByLoginId).toHaveBeenCalledWith(123);

    // Verify the error message was set
    expect(component.errorMessage).toBe('Failed to load tasks.');
  });

  it('should toggle task completion status', () => {
    const mockTask: Task = {
      taskId: 1,
      taskName: 'Test Task',
      note: '',
      dayOfWeek: 'Monday',
      isCompleted: false,
      taskOrder: 1,
      loginId: 123,
      roomies: [],
    };


    taskService.updateTask.and.returnValue(of(mockTask));


    component.toggleTaskCompletion(mockTask);


    expect(mockTask.isCompleted).toBe(true);


    expect(taskService.updateTask).toHaveBeenCalledWith(
      jasmine.objectContaining({ isCompleted: true }),
      []
    );
  });
});


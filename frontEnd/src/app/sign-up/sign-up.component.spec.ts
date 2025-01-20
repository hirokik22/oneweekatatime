import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SignUpComponent } from './sign-up.component';
import { SignUpService } from '../services/sign-up.service';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';

describe('SignUpComponent', () => {
  let component: SignUpComponent;
  let fixture: ComponentFixture<SignUpComponent>;
  let signUpService: jasmine.SpyObj<SignUpService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const signUpServiceSpy = jasmine.createSpyObj('SignUpService', ['signUp']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [
        SignUpComponent, // Import SignUpComponent directly as a standalone component
        HttpClientTestingModule, // For HttpClient dependency
      ],
      providers: [
        { provide: SignUpService, useValue: signUpServiceSpy },
        { provide: Router, useValue: routerSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SignUpComponent);
    component = fixture.componentInstance;
    signUpService = TestBed.inject(SignUpService) as jasmine.SpyObj<SignUpService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should show an error if passwords do not match', () => {
    component.password = 'password123';
    component.rePassword = 'password456';

    component.signUp();

    expect(component.errorMessage).toBe('Passwords do not match.');
  });

  it('should call signUp service and navigate on success', () => {
    const mockResponse = { loginId: 123 };
    signUpService.signUp.and.returnValue(of(mockResponse));

    component.email = 'test@example.com';
    component.password = 'password123';
    component.rePassword = 'password123';
    component.roomie1 = 'Alice';
    component.roomie2 = 'Bob';

    component.signUp();

    expect(signUpService.signUp).toHaveBeenCalledWith({
      email: 'test@example.com',
      passwordHash: 'password123',
      roomieNames: ['Alice', 'Bob'],
    });
    expect(sessionStorage.getItem('loginId')).toBe('123');
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should handle sign-up error and display error message', () => {
    const mockError = { message: 'Signup failed' };
    signUpService.signUp.and.returnValue(throwError(() => mockError));

    component.email = 'test@example.com';
    component.password = 'password123';
    component.rePassword = 'password123';

    component.signUp();

    expect(signUpService.signUp).toHaveBeenCalled();
    expect(component.errorMessage).toBe('Signup failed');
  });

  it('should filter out empty roomie names before sign-up', () => {
    signUpService.signUp.and.returnValue(of({ loginId: 123 }));

    component.email = 'test@example.com';
    component.password = 'password123';
    component.rePassword = 'password123';
    component.roomie1 = 'Alice';
    component.roomie2 = '';
    component.roomie3 = 'Charlie';
    component.roomie4 = '';

    component.signUp();

    expect(signUpService.signUp).toHaveBeenCalledWith({
      email: 'test@example.com',
      passwordHash: 'password123',
      roomieNames: ['Alice', 'Charlie'],
    });
  });
});

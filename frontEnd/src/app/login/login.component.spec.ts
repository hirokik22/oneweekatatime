import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { LoginService } from '../services/login.service';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let loginService: jasmine.SpyObj<LoginService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const loginServiceSpy = jasmine.createSpyObj('LoginService', ['Login']);
    const routerSpyObj = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, LoginComponent],
      providers: [
        { provide: LoginService, useValue: loginServiceSpy },
        { provide: Router, useValue: routerSpyObj },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    loginService = TestBed.inject(LoginService) as jasmine.SpyObj<LoginService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should call the LoginService on login success and navigate to tasks', () => {
    const mockResponse = { loginId: 123 }; // Mock backend response
    loginService.Login.and.returnValue(of(mockResponse));

    // Set form values
    component.credentials.email = 'test@example.com';
    component.credentials.passwordHash = 'password123';

    component.login();

    expect(loginService.Login).toHaveBeenCalledWith(component.credentials); // Ensure the service is called with the right credentials
    expect(sessionStorage.getItem('loginId')).toBe('123'); // Check if loginId is stored
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/tasks']); // Ensure navigation occurs
  });

  it('should handle login error and set an error message', () => {
    const mockError = { message: 'Invalid email or password.' };
    loginService.Login.and.returnValue(throwError(() => mockError));

    component.credentials.email = 'test@example.com';
    component.credentials.passwordHash = 'wrongpassword';

    component.login();

    expect(loginService.Login).toHaveBeenCalledWith(component.credentials);
    expect(component.errorMessage).toBe('Invalid email or password. Please try again.');
  });

  it('should navigate to sign-up page when navigateToSignUp is called', () => {
    component.navigateToSignUp();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/sign-up']);
  });
});

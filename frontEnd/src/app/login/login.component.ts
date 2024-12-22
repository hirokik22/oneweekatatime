import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../services/login.service';
import { Login } from '../model/login';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports: [FormsModule, CommonModule],
})
export class LoginComponent {
  // Login model bound to the form
  credentials: Login = {
    email: '',
    passwordHash: '',
  };

  errorMessage: string = ''; // To display login errors

  constructor(private loginService: LoginService, private router: Router) {}

  // Handle form submission for login
  login() {
    this.errorMessage = ''; // Clear any previous error messages
    this.loginService.Login(this.credentials).subscribe(
      (response: any) => {
        console.log('Login successful:', response);
  
        // Store loginId in session storage
        if (response.loginId) {
          console.log('Storing loginId:', response.loginId);
          sessionStorage.setItem('loginId', response.loginId.toString());
          console.log('LoginId stored:', response.loginId);
        }
  
        alert('Login successful!');
        this.router.navigate(['/tasks']); // Navigate to tasks page on success
      },
      (error) => {
        console.error('Login failed:', error);
        this.errorMessage = 'Invalid email or password. Please try again.';
      }
    );
  }
  

  // Handle navigation to sign-up page
  navigateToSignUp() {
    this.router.navigate(['/sign-up']); // Replace with the actual sign-up route
  }
}
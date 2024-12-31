import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { SignUpService } from '../services/sign-up.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sign-up',
  standalone:true,
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css'],
  imports: [FormsModule, CommonModule],
})
export class SignUpComponent {
  email: string = '';
  password: string = '';
  rePassword: string = '';
  roomie1: string = '';
  roomie2: string = '';
  roomie3: string = '';
  roomie4: string = '';
  errorMessage: string = '';

  constructor(private signUpService: SignUpService, private router: Router) {}


  signUp() {
    if (this.password !== this.rePassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }
  
    const signUpData = {
      email: this.email,
      passwordHash: this.password,
      roomieNames: [this.roomie1, this.roomie2, this.roomie3, this.roomie4].filter(name => name)
    };
  
    this.signUpService.signUp(signUpData).subscribe({
      next: (response: any) => {
        console.log('Signup successful:', response);
  
        // Store loginId in session storage
        if (response.loginId) {
          sessionStorage.setItem('loginId', response.loginId.toString());
          console.log('LoginId stored:', response.loginId);
        }
  
        this.errorMessage = ''; // Clear any error message
        alert('Signup successful!');
  
        // Navigate to the tasks page
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Signup failed:', err);
        this.errorMessage = err.message || 'Signup failed. Please try again.';
      }
    });
  }  
}
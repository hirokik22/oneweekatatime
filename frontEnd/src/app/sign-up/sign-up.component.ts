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
  
    const signUpPayload = {
      email: this.email,
      passwordHash: this.password,
      roomieNames: [
        this.roomie1,
        this.roomie2,
        this.roomie3,
        this.roomie4,
      ].filter((name) => name), // Filter out empty roomie names
    };
  
    this.signUpService.signUp(signUpPayload).subscribe({
      next: (response) => {
        console.log('Sign-up successful:', response);
        this.router.navigate(['/tasks']); // Navigate to tasks after successful sign-up
      },
      error: (error) => {
        if (error.status === 409) {
          // Backend indicates email already exists
          this.errorMessage = 'This account already exists, please login.';
        } else {
          // Generic error message
          this.errorMessage = 'Sign-up failed. Please try again.';
        }
      },
    });
  }  
}
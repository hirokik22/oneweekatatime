import { Component } from '@angular/core';
import { Routes } from '@angular/router';
import { TaskComponent } from './task/task.component';
import { LoginComponent } from './login/login.component';
import { SignUpComponent } from './sign-up/sign-up.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'tasks', component: TaskComponent },  
  { path: 'login', component: LoginComponent }, // LoginComponent route
  { path: 'sign-up', component: SignUpComponent }, // SignUpComponent route
];
import { Component } from '@angular/core';
import { Routes } from '@angular/router';
import { TaskComponent } from './task/task.component';
import { LoginComponent } from './login/login.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { PlannerComponent } from './planner/planner.component';

export const routes: Routes = [
  { path: '', redirectTo: '/tasks', pathMatch: 'full' }, // Default route
  { path: 'tasks', component: TaskComponent }, // TaskComponent route
  { path: 'login', component: LoginComponent }, // LoginComponent route
  { path: 'sign-up', component: SignUpComponent }, // SignUpComponent route
  { path: 'planner', component: PlannerComponent},
];
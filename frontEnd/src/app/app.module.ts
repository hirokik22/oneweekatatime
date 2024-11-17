import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { LoginComponent } from './login/login.component';
import { AppComponent } from './app.component';

@NgModule({
    declarations: [
        LoginComponent
    ],
  imports: [BrowserModule, ReactiveFormsModule],
  providers: [],
})
export class AppModule {}

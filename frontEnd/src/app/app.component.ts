import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router'; // Import RouterOutlet

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet], // Add RouterOutlet here
  template: `<router-outlet></router-outlet>`, // Routing placeholder
  styleUrls: ['./app.component.css'],
})
export class AppComponent {}
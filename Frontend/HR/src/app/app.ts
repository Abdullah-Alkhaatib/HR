import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { RandomColor } from './directives/random-color';
import { ReactiveFormsModule } from '@angular/forms';
import { Employees } from './components/employees/employees';
import { Departments } from './components/departments/departments';

@Component({ // Decorator
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RandomColor, ReactiveFormsModule, Employees, Departments, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  
}

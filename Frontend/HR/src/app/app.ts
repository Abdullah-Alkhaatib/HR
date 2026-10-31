import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RandomColor } from './directives/random-color';
import { ReactiveFormsModule } from '@angular/forms';
import { Employees } from './components/employees/employees';

@Component({ // Decorator
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RandomColor, ReactiveFormsModule, Employees],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  
}

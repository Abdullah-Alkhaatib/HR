import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RandomColor } from './directives/random-color';
import { ReactiveFormsModule } from '@angular/forms';

@Component({ // Decorator
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RandomColor, ReactiveFormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  // public readonly title = signal('Welcome to Angular From Typescript');

  // title: string = "Welcome to Angular From Typescript";
  // number: number = 52.4549;
  // bool: boolean = true;
  // arr: string[] = ['1', '2', '3', '4', '5'];

  stu =[
    { name: 'stu_1', mark: 49 },
    { name: 'stu_2', mark: 90 },
    { name: 'stu_3', mark: 90 },
    { name: 'stu_4', mark: 90 },
    { name: 'stu_5', mark: 90 },
    { name: 'stu_6', mark: 90 },
  ]
  
}

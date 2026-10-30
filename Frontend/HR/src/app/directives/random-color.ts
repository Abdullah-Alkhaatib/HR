import { Directive, ElementRef } from '@angular/core';

@Directive({
  selector: '[appRandomColor]',
})
export class RandomColor {

  constructor(private el: ElementRef) {

    const colors = ['red', 'blue', 'green', "purple", "orange", "yellow"];

    let color = colors[Math.floor(Math.random() * colors.length)];
    this.el.nativeElement.style.backgroundColor = color;
  }

}

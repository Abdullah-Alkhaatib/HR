import { Component, EventEmitter, input, Input, Output } from '@angular/core';

@Component({
  imports: [],
  selector: 'app-confirmation-dialog',
  styleUrl: './confirmation-dialog.css',
  templateUrl: './confirmation-dialog.html',
})
export class ConfirmationDialog {

  @Input() title: string = ""; // Get value from parent component
  @Input() body: string = "";

  @Output() confirm = new EventEmitter<boolean>();

  confirmDelete(isConfirmed: boolean) {
    this.confirm.emit(isConfirmed); // Activate confirm event, and transfer value to parent component
  }

}

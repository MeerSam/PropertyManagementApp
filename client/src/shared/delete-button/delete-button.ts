import { Component, input, output } from '@angular/core';


@Component({
  selector: 'app-delete-button',
  imports: [],
  templateUrl: './delete-button.html',
  styleUrl: './delete-button.css',
})
export class DeleteButton {
  disabled = input<boolean>(false); //input property
  clickEvent = output<Event>();  // for the button clicked


  onClick(event: Event) {
    this.clickEvent.emit(event);
  }
}

import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RegisterDto } from '../../../types/auth';
import { Client } from '../../../types/client'; 
import { SessionService } from '../../../core/services/session-service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  protected session = inject(SessionService) 

  clientFromHome = input.required<Client | null>();
  protected creds = {} as RegisterDto;
  protected currentStep = signal(1);
  protected validationErrors = signal<string[]>([]);
  cancelRegister = output<boolean>(); 

  getListOfClients(): Client[] {
    const activeClient = this.session.activeClient();

    // Wrap a single client into an array
    const ensureArray = (x: Client | Client[]) => Array.isArray(x) ? x : [x];

    // If we have an active client, return it as a list
    if (activeClient) {
      return ensureArray(activeClient);
    }

    // Otherwise return available clients (already an array)
    return this.session.availableClients();
  }

  register() {
    console.log(this.creds);
  }

  cancel() {
    console.log('cancelled register');
    this.cancelRegister.emit(false);
  }

}

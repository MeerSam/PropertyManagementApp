import { Component, inject, input, output, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RegisterDto } from '../../../types/user';
import { Client } from '../../../types/client';
import { ValidationError } from '@angular/forms/signals';
import { TenantService } from '../../../core/services/tenant-service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  protected tenantService = inject(TenantService) 

  clientFromHome = input.required<Client | null>();
  protected creds = {} as RegisterDto;
  protected currentStep = signal(1);
  protected validationErrors = signal<string[]>([]);
  cancelRegister = output<boolean>(); 

  getListOfClients(): Client[] {
    const activeClient = this.tenantService.activeClient();

    // Wrap a single client into an array
    const ensureArray = (x: Client | Client[]) => Array.isArray(x) ? x : [x];

    // If we have an active client, return it as a list
    if (activeClient) {
      return ensureArray(activeClient);
    }

    // Otherwise return available clients (already an array)
    return this.tenantService.availableClients();
  }

  register() {
    console.log(this.creds);
  }

  cancel() {
    console.log('cancelled register');
    this.cancelRegister.emit(false);
  }

}

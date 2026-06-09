import { Component, computed, effect, inject, input, OnInit, output, signal } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RegisterDto } from '../../../types/auth';
import { Client } from '../../../types/client';
import { SessionService } from '../../../core/services/session-service';
import { JsonPipe } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { SelectInput } from "../../../shared/select-input/select-input";
import { SelectOption } from '../../../types/select';
import { maxLength } from '@angular/forms/signals';
import { TextInput } from "../../../shared/text-input/text-input";
import { APP_ROLE, APP_ROLE_LABELS } from '../../../types/user';
import { Router } from '@angular/router';
import { ToastService } from '../../../core/services/toast-service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, JsonPipe, SelectInput, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {

  protected session = inject(SessionService)
  protected router = inject(Router)
  protected toast = inject(ToastService)


  clientFromHome = input.required<Client | null>();
  cancelRegister = output<boolean>();
  protected registerForm: FormGroup = new FormGroup({})
  protected profileForm: FormGroup = new FormGroup({})


  protected creds = {} as RegisterDto;

  protected currentStep = signal(1);
  protected validationErrors = signal<string[]>([]);
  // Track whether user manually edited displayName
  private userEditedDisplayName = signal(false);
  protected roleOptions = computed<SelectOption[]>(() => {
    const currentUserRole = this.session.currentUser()?.role;
    if (currentUserRole == 'admin') {
      return APP_ROLE
        .map(r => ({ label: APP_ROLE_LABELS[r], value: r }))
    } else if (currentUserRole == 'property_manager') {
     return [
        { label: 'owner', value: 'owner' }, 
        { label: 'Board Member', value: 'board_member' }, 
        { label: 'resident', value: 'resident' }];
    }
    return [
        { label: 'Owner', value: 'owner' }, 
        { label: 'Resident', value: 'resident' }];
  })


  initializeForm() {
    this.registerForm = new FormGroup({
      // define form controls that we will use in here
      clientId: new FormControl(this.session.activeClient()?.clientId, Validators.required),
      email: new FormControl('jondoe@test.com', [Validators.required, Validators.email]),
      firstName: new FormControl('', Validators.required),
      lastName: new FormControl('', Validators.required),
      displayName: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
    })

    this.registerForm.controls['password'].valueChanges.subscribe(() => {
      this.registerForm.controls['confirmPassword'].updateValueAndValidity();
    })

    this.profileForm = new FormGroup({
      gender: new FormControl('male', [Validators.required]),
      dateOfBirth: new FormControl('', Validators.required),
      role: new FormControl('', Validators.required) 
    });

  }
  constructor() {
    this.initializeForm();
    this.setupAutoDisplayName()
  }

  private setupAutoDisplayName() {
    const firstNameSig = toSignal(this.registerForm.controls['firstName'].valueChanges, { initialValue: '' });
    const lastNameSig = toSignal(this.registerForm.controls['lastName'].valueChanges, { initialValue: '' });

    // Detect manual edits
    this.registerForm.controls['displayName'].valueChanges.subscribe(() => {
      this.userEditedDisplayName.set(true);
    });

    // Auto-update effect
    effect(() => {
      if (this.userEditedDisplayName()) return;

      const first = firstNameSig() ?? '';
      const last = lastNameSig() ?? '';

      this.registerForm.controls['displayName'].setValue(
        `${first} ${last}`.trim(),
        { emitEvent: false }
      );
    });
  }


  protected getListOfClients(): Client[] {
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
    if (this.registerForm.valid && this.profileForm.valid) {
      const formData = { ...this.registerForm.value, ...this.profileForm.value };
      // console.log(formData)
      this.session.register(formData).subscribe({
        next: result => {
          // console.log(result)
          if (result.id) {
            this.router.navigateByUrl('/members/' + result.id);
          }
          this.router.navigateByUrl('/members');
        },
        error: err => {
          // console.log('New Registration for user was not completed', err);
          this.toast.error(`Error while creating new user ${this.registerForm.controls['displayName'].value}. ${err}`)
        }
      });
    }
  }

  cancel() {
    // //*meera console.log(.log('cancelled register');
    this.cancelRegister.emit(false);
  }
  nextStep() {
    if (this.registerForm.valid) {
      this.currentStep.update(prevStep => prevStep + 1);
    }
  }

  prevStep() {
    this.currentStep.update(prevStep => prevStep - 1);
  }

  getClientOptions(): SelectOption[] {
    const clients = this.getListOfClients();
    let clientOptions: SelectOption[] = [{
      value: 'null',
      label: 'Pick a client'
    }];

    if (clients) {
      const options = clients
        .filter(c => c.isActiveClient)
        .map(c => ({
          value: c.clientId,
          label: c.clientName
        }))
      clientOptions = [...options]
    }
    return clientOptions;
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const parent = control.parent; //control.parent  is the formGroup= this.registerForm

      if (!parent) return null;
      const matchValue = parent.get(matchTo)?.value;
      return control.value === matchValue ? null : { passwordMismatch: true }
    }
  }
  getMaxDate() {
    const today = new Date();
    today.setFullYear(today.getFullYear() - 18);
    return today.toISOString().split('T')[0];
  }



}

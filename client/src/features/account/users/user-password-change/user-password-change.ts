import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { SessionService } from '../../../../core/services/session-service';
import { TextInput } from "../../../../shared/text-input/text-input";
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../../../types/user';
import { ToastService } from '../../../../core/services/toast-service';

@Component({
  selector: 'app-user-password-change',
  imports: [TextInput, ReactiveFormsModule],
  templateUrl: './user-password-change.html',
  styleUrl: './user-password-change.css',
})
export class UserPasswordChange implements OnInit, OnDestroy {
  protected session = inject(SessionService)
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private toast = inject(ToastService);

  protected user = signal<User | null>(null);

  protected credentialsForm: FormGroup;
  constructor() {
    this.credentialsForm = this.fb.group({
      email: [this.user()?.email ?? '' , Validators.required],
      userId: [this.user()?.id?? '' , Validators.required],
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required,
                      Validators.minLength(4),
                      Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('newPassword')]]
    });
  }
  ngOnInit(): void {
    this.route.parent?.data.subscribe(data => {
      this.user.set(data['user'])
    })
  }
  ngOnDestroy(): void {
    if (this.session.editMode()) {
      // console.log('resetting editmode to false... current value', this.session.editMode())
      this.session.editMode.set(false);

    }
  }
  cancel() {
    this.credentialsForm.reset()
  }
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const parent = control.parent; //control.parent  is the formGroup= this.registerForm

      if (!parent) return null;
      const matchValue = parent.get(matchTo)?.value;
      return control.value === matchValue ? null : { passwordMismatch: true }
    }
  }

  changeCredentials(){
    if (this.credentialsForm.valid){
      const formData={...this.credentialsForm.value}
      // console.log(formData);
      this.session.updateCredentials(formData).subscribe({
        next: () => {
          this.toast.success('Profile Updated for user succesfully');
          this.session.editMode.set(false);
          this.user.set(formData as User);
          if (formData.userId == this.session.currentUser()?.id &&
           formData.displayName !== this.session.currentUser()?.displayName  ) {
            this.session.currentUser.update(u => ({ ...u!, displayName: formData.displayName }));
          }
        },
        error: error => this.toast.error('Error while saving.' + error)
      });
    }
  }

}

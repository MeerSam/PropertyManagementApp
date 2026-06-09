import { Component, inject } from '@angular/core';
import { SessionService } from '../../../core/services/session-service';
import { Router } from '@angular/router';
import { ToastService } from '../../../core/services/toast-service';
import { FormBuilder, FormControl, FormGroup, Validators, ɵInternalFormsSharedModule } from '@angular/forms';
import { email, validate } from '@angular/forms/signals';
import { environment } from '../../../environments/environment';
import { Location } from '@angular/common';

@Component({
  selector: 'app-forgot-password',
  imports: [ɵInternalFormsSharedModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
})
export class ForgotPassword {
  protected session = inject(SessionService)
  protected router = inject(Router)
  protected toast = inject(ToastService);
  private fb = inject(FormBuilder);
  private location = inject(Location);
  protected passwordChangeForm: FormGroup = new FormGroup({});


  constructor() {
    this.passwordChangeForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    })
  }

  submit() {
    if (this.passwordChangeForm.valid) return this.toast.error("Invalid input")
    const email = this.passwordChangeForm.value.email!;
    const baseUrl = window.location.origin;
    window.location.host
    const payload = {
      email,
      resetUrlBase: `${baseUrl}/reset-password`
    }
    this.session.forgotPassword(payload).subscribe({
      next: result => {
         this.toast.error("Check Email for a link")
        this.router.navigateByUrl("/home");
      },
      error: error => this.toast.error("Could not complete request")
    })
  }
}

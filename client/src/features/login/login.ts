import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SessionService } from '../../core/services/session-service';
import { Router } from '@angular/router';
import { ToastService } from '../../core/services/toast-service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  protected session = inject(SessionService);
  private toastService = inject(ToastService)

  protected loginForm: FormGroup;
  protected selectClientForm: FormGroup;


  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });

    this.selectClientForm = this.fb.group({
      client: ['', [Validators.required]]
    });
  }

  login() {
    if (this.loginForm.invalid) return;

    const creds = this.loginForm.value;
    console.log('In login: ')
    this.session.login(creds).subscribe({
      next: outcome => {
        switch (outcome?.status) {
          case 'success':
            this.toastService.info("successfully logged in...")
            this.router.navigateByUrl('/dashboard');
            break;
          case 'needs_client_selection':
            this.toastService.info("successfully logged in. Choose client")
            this.router.navigateByUrl('/select-client');
            break;

          case 'error':
            this.toastService.error(outcome?.message)
            break;
          default:
            this.toastService.error('Unknown error during login process')
            break;
        }
      },
      error: error => this.toastService.error(error.error),
      complete: () => console.log("Login completed")
    });
  }

  selectClient() {
    if (this.selectClientForm.invalid) return;
    const clientchoice = this.selectClientForm.value;

  }

  logout() {
    this.session.logout()
  }

}

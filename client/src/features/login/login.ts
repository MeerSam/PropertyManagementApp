import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { SessionService } from '../../core/services/session-service';
import { TenantService } from '../../core/services/tenant-service';
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
  protected accountService = inject(AccountService);
  protected tenantService = inject(TenantService);
  protected sessionService = inject(SessionService);
  private toastService =inject(ToastService)

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
    this.sessionService.loginAndSelectClient(creds).subscribe({
      next: result => {
        // navigate to Dashboard 
        if(this.accountService.isAuthSuccess(result)){
          // this.router.navigateByUrl('/dashboard');
          this.toastService.info("successfully logged in")

        }else if(this.accountService.isClientSelect(result)) { 
          // show step2
          this.toastService.info("successfully logged in. Choose client")
        }
        else{
          this.toastService.error(result.message)  
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

}

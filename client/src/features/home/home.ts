import { Component, inject, Input, signal } from '@angular/core';
import { AccountService } from '../../core/services/account-service';
import { Login } from "../login/login";
import { Client } from '../../types/client';
import { Register } from "../account/register/register";
import { SessionService } from '../../core/services/session-service';
import { TenantService } from '../../core/services/tenant-service';

@Component({
  selector: 'app-home',
  imports: [Login, Register],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  @Input({required: true}) clientFromApp : Client |null = null;
  protected accountService = inject(AccountService);
  protected tenantService = inject(TenantService);
  protected registerMode = signal(false);

  showRegister(value: boolean){
    console.log('value of event: ' + value);
    this.registerMode.set(value); 
  }
  

}

import { Component, inject, Input, signal } from '@angular/core'; 
import { Login } from "../login/login";
import { Client } from '../../types/client';
import { Register } from "../account/register/register";
import { RouterLink } from '@angular/router';
import { SessionService } from '../../core/services/session-service';

@Component({
  selector: 'app-home',
  imports: [Login, Register, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home   {
  
  @Input({required: true}) clientFromApp : Client | null = null;
  protected session = inject(SessionService);  
  protected registerMode = signal(false); 

  showRegister(value: boolean){
    console.log('show register value of event: ' + value);
    this.registerMode.set(value); 
  }
  

}

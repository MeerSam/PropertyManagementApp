import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Nav } from "../layout/nav/nav";
import { environment } from '../environments/environment.development';
import { AccountService } from '../core/services/account-service';
import { Home } from "../features/home/home";
import { TenantService } from '../core/services/tenant-service';
import { Client } from '../types/client';

@Component({
  selector: 'app-root',
  imports: [Nav, Home],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {

  private http = inject(HttpClient);
  protected accountService = inject(AccountService)
  protected tenantService = inject(TenantService) 
  protected readonly title = signal('Property Management');
  protected members = signal<any>([]);
  
  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
    if (this.tenantService.activeClient() == null) {
      this.setCurrentUserClient();
    }

  }
  setCurrentUserClient() {
    const clientString = localStorage.getItem('activeClient');
    if (!clientString) return;
    const client = clientString? JSON.parse(clientString) as Client : null;
    if(!client) alert('Client Missing')
    this.tenantService.setActiveClient(client);
  }

}

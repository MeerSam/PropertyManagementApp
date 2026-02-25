import { inject, Injectable } from '@angular/core';
import { AccountService } from './account-service';
import { TenantService } from './tenant-service';
import { Client } from '../../types/client';
import { ToastService } from './toast-service';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  protected accountService = inject(AccountService)
  protected tenantService = inject(TenantService)
  protected toast = inject(ToastService)


  init(): Observable<null> {
    const userString = localStorage.getItem('user');
    const clientString = localStorage.getItem('activeClient');

    if (!userString || !clientString) return of(null);
    // set the current user
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
    // set current client
     const client = clientString ? JSON.parse(clientString) as Client : null;
    if (!client) {     
      this.tenantService.activeClient.set(client)
      this.tenantService.setActiveClient(client);
    }
    return of(null) //Observable<null>
  }
}

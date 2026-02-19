import { inject, Injectable, signal } from '@angular/core';
import { AccountService } from './account-service';
import { TenantService } from './tenant-service';
import { Client } from '../../types/client';
import { LoginCreds, SelectClientDto } from '../../types/user';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  private accountService = inject(AccountService);
  private tenantService = inject(TenantService); 

  loginAndSelectClient(creds: LoginCreds) {
    return this.accountService.login(creds).pipe(
      tap(response => {
        if (this.accountService.isAuthSuccess(response)) {
          this.accountService.setClientUserAfterLogin(response);
        }
        if (this.accountService.isClientSelect(response)) {
          // Needs User Confirmation on which client they want to choose for this session.
          this.tenantService.setSelectionToken(JSON.stringify(response.selectionToken)); 
          localStorage.setItem('availableClients', JSON.stringify(response.availableClients))
        }
        if (this.accountService.isAuthError(response)) {
          console.error(response.message);
          return;
        }
      })
    );
  }

  finalizeClientSelection(selectedClient: Client) {
    if ((!this.tenantService.selectionToken())) return alert('Cannot complete request. Select Client Token missing'); 
    const creds: SelectClientDto = {
      clientId: selectedClient.clientId,
      selectionToken: this.tenantService.selectionToken()
    };

    this.tenantService.selectClient(creds).pipe(
      tap(response => {
        if (this.accountService.isAuthSuccess(response)) {
          this.accountService.setClientUserAfterLogin(response);
          this.tenantService.setActiveClient(selectedClient);
        }
        if (this.accountService.isAuthError(response)) {
          console.error(response.message);
          return;
        }
      })
    )
    
  }

  fullLogout() {
    this.tenantService.clear();
    this.accountService.logout();
  }






}

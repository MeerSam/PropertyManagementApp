import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account-service';
import { ToastService } from '../services/toast-service';
import { TenantService } from '../services/tenant-service';
import { SessionService } from '../services/session-service';

export const authGuard: CanActivateFn = () => {
  // Guards helps us keep our routes secure 
  // only if the function returns true the page or route can be accessed.
  const accountService = inject(AccountService);
  const tenantService = inject(TenantService);
  const toast = inject(ToastService);

  if (accountService.currentUser() && tenantService.activeClient()) return true;
  else {
    toast.error('You shall not pass');
    return false;
  }
  //removed (route, state) as its not needed here
};

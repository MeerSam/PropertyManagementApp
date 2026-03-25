import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { ToastService } from '../services/toast-service';
import { SessionService } from '../services/session-service';

export const authGuard: CanActivateFn = () => {
  // Guards helps us keep our routes secure 
  // only if the function returns true the page or route can be accessed. 
  const session = inject(SessionService);
  const toast = inject(ToastService);

  if (session.currentUser() && session.activeClient()) return true;
  else {
    toast.error('You shall not pass');
    return false;
  }
  //removed (route, state) as its not needed here
};

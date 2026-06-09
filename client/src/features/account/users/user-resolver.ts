import { ResolveFn, Router } from '@angular/router';
import { User } from '../../../types/user';
import { SessionService } from '../../../core/services/session-service';
import { inject } from '@angular/core';
import { EMPTY } from 'rxjs';

export const userResolver: ResolveFn<User> = (route, state) => {
  const session = inject(SessionService);
  const userId = route.paramMap.get('id');
  const router = inject(Router);
  if (!userId) {
    router.navigateByUrl('/not-found');
    return EMPTY;
  }
  return session.loadUserForEdit(userId); 
};

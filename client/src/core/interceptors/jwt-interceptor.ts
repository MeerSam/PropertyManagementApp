import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { SessionService } from '../services/session-service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const session = inject(SessionService)
  const user = session.currentUser();

  // modify the request by cloning as the req is immutable
  if(user){
    req = req.clone({
      setHeaders:{
        Authorization:  `Bearer ${user.accessToken}`
      }
    })
  }
  return next(req);
};

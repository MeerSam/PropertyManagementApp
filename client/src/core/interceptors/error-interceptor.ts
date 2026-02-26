import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError } from 'rxjs';
import { ToastService } from '../services/toast-service';
import { NavigationExtras, Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);
  const router = inject(Router);

  return next(req).pipe(
    catchError(error => {
      if (error) {
        switch (error.status) {
          case 400:
            if(error.error.errors){
              const modelStateError =[];
              for (const key in error.error.errors) {
                 if(error.error.errors[key]){
                  modelStateError.push(error.error.errors[key])
                 } 
              }
              throw modelStateError.flat()
            }else{
              toast.error(error.error)
            } 
            break;
          case 401:
            toast.error("Unauthorized");
            break;
          case 404:
            toast.error("Not Found");
            router.navigateByUrl('/not-found')
            break;
          case 500:
            const navigationExtras : NavigationExtras = {state:{error: error.error}}
            router.navigateByUrl('/server-error', navigationExtras)
            break;
          case 405:
            toast.error(error.error);
            break;
          default:
            toast.error(`Something went wrong ${error.status}`);
            break;
        }
      }
      throw error;
    })
  );
};

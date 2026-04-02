import { HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, of, tap } from 'rxjs';


const cache = new Map<string, HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);
  busyService.busy();

  if(req.method === 'GET'){
    const cachedResponse = cache.get(req.url);
    if(cachedResponse){
      return of(cachedResponse);
    }
  }
  //after the request comes back in the response(next(req)) below we need to add a fake delay
  return next(req).pipe(
    delay(500),
    tap(response =>{
      cache.set(req.url, response)
    }),
    finalize(()=> {
      busyService.idle()
    })
  );
};

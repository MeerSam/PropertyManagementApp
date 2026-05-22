import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, of, tap } from 'rxjs';


const cache = new Map<string, HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  //method to create a cache key
  const generateCacheKey = (url: string, params: HttpParams): string => {
    const paramString = params.keys().map(key => `${key}=${params.get(key)}`).join('&');

    return paramString ? `${url}?${paramString}` : url;
  }

  //method: invalidateCache => we're effectively going to loop over our cache keys. 
  // And we're going to look for a pattern that matches the URL inside our cache keys, and if it finds it
  // based on what we passed to this method, then we're going to delete that cache key.
  // And then the next time a request comes in because that's not going to be available inside the cache.
  // That's going to force us to go out to the API to get fresh data.
  const invalidateCache = (urlPattern: string) => {
    for (const key of cache.keys()) {
      if (key.includes(urlPattern)) {
        cache.delete(key);
      }
    }

  }

  const cacheKey = generateCacheKey(req.url, req.params);
  if (req.method.includes('POST') || req.method.includes('PUT') || req.method.includes('PATCH') || req.method.includes('DELETE')) {
    if (req.url.includes('/members')) {
      invalidateCache('/members')
    }
    if (req.url.includes('/properties')) {
      invalidateCache('/properties')
    }
    if (req.url.includes('/account')) {
      invalidateCache('/account')
    }
  }


  if (req.method === 'GET') {
    console.log(cacheKey);
    const cachedResponse = cache.get(cacheKey);
    if (cachedResponse) {
      return of(cachedResponse);
    }
  }

  busyService.busy();

  //after the request comes back in the response(next(req)) below we need to add a fake delay
  return next(req).pipe(
    delay(500),
    tap(response => {
      cache.set(req.url, response)
    }),
    finalize(() => {
      busyService.idle()
    })
  );
};

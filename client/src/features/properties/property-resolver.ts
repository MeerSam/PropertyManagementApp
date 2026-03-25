import { ResolveFn, Router } from '@angular/router';
import { PropertyService } from '../../core/services/property-service';
import { inject } from '@angular/core';
import { EMPTY } from 'rxjs';
import { Property } from '../../types/property';

export const propertyResolver: ResolveFn<Property> = (route, state) => {
  const propertyService = inject(PropertyService)
  const propertyId = route.paramMap.get('id');
  const router = inject(Router);

  if (!propertyId) {
    router.navigateByUrl('/not-found');
    return EMPTY;
  }
  return propertyService.getProperty(propertyId);
};

import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Property } from '../../types/property';
import { tap } from 'rxjs'; 

@Injectable({
  providedIn: 'root',
})
export class PropertyService {
  private http = inject(HttpClient);  
  private baseUrl = environment.apiUrl;
  property = signal<Property | null>(null);

  getProperty(id: string) {
    return this.http.get<Property>(this.baseUrl + 'properties/' + id).pipe(
      tap(property => {
        this.property.set(property)
      })
    )
  }

  getPropertiesByClient(clientId: string) {
    return this.http.get<Property[]>(this.baseUrl + 'properties' );
  }

  getPropertiesByOwner(memberId: string) { 
    return this.http.get<Property[]>(this.baseUrl + 'members/' + memberId + '/properties') ;
  } 
}

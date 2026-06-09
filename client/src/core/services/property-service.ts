import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { EditableProperty, Property } from '../../types/property';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PropertyService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  property = signal<Property | null>(null);
  editMode = signal(false);

  getProperty(id: string) {
    this.editMode.set(false);
    return this.http.get<Property>(this.baseUrl + 'properties/' + id).pipe(
      tap(property => {
        // console.log('at get property', property.isSameAddress)

        this.property.set(property)
      })
    )
  }

  getPropertiesByClient(clientId: string) {
    return this.http.get<Property[]>(this.baseUrl + 'properties');
  }

  getPropertiesByOwner(memberId: string) {
    return this.http.get<Property[]>(this.baseUrl + 'members/' + memberId + '/properties');
  }

  updateProperty(property: EditableProperty) {
    // console.log("property=property", property)
    return this.http.put(this.baseUrl + 'properties/' + this.property()?.id, property);
  }
}

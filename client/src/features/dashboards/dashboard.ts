import { Component, inject, signal } from '@angular/core';
import { PropertyService } from '../../core/services/property-service';
import { PropertyCard } from '../properties/property-card/property-card';
import { Property } from '../../types/property';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { SessionService } from '../../core/services/session-service';

@Component({
  selector: 'app-dashboard',
  imports: [PropertyCard, AsyncPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard   { 
  protected session = inject(SessionService)
  protected propertyService = inject(PropertyService);
  protected properties$!: Observable<Property[]>;
  protected announcements = signal<string[]|null>([])

  constructor()  {
    const memberId = this.session.activeClient()?.memberId
    console.log(memberId)
    if (memberId) {
      this.properties$ = this.propertyService.getPropertiesByOwner(memberId);
    }

  }

  // loadOwnerProperties(memberId: string) {
  //   this.propertyService.getPropertiesByOwner(memberId).subscribe({
  //     next: result => {
  //       this.properties$.set(result);
  //     },
  //     error: error => console.log(error)
  //   })
  // }

}

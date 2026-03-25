import { Component, inject, signal } from '@angular/core';
import { SessionService } from '../../../core/services/session-service';
import { PropertyService } from '../../../core/services/property-service';
import { Property } from '../../../types/property';
import { Observable } from 'rxjs';
import { PropertyCard } from "../../properties/property-card/property-card";
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-dashboard-owner',
  imports: [PropertyCard, AsyncPipe],
  templateUrl: './dashboard-owner.html',
  styleUrl: './dashboard-owner.css',
})
export class DashboardOwner {
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
}

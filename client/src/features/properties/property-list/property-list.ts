import { Component, inject, OnInit, signal } from '@angular/core';
import { PropertyService } from '../../../core/services/property-service';
import { Property } from '../../../types/property';
import { PropertyCard } from '../property-card/property-card';
import { SessionService } from '../../../core/services/session-service';
import { Observable } from 'rxjs';
import { ToastService } from '../../../core/services/toast-service';

@Component({
  selector: 'app-property-list',
  imports: [PropertyCard],
  templateUrl: './property-list.html',
  styleUrl: './property-list.css',
})
export class PropertyList implements OnInit {

  protected propertyService = inject(PropertyService);
  private session = inject(SessionService)
  protected properties = signal<Property[] | null>([]); 


  ngOnInit(): void {
    this.loadProperties();
  }



  loadProperties() {
    // admin & property manager view
    const clientId = this.session.activeClient()?.clientId;
    const memberId = this.session.activeClient()?.memberId
    var observableProperties$: Observable<Property[]> | undefined;
    if (!!clientId ) {
      switch (this.session.currentRole()) {
        case 'admin':
          observableProperties$ = this.propertyService.getPropertiesByClient(clientId);
          break;
        case 'owner':
        case 'board_member':
          if (memberId != undefined)
            observableProperties$ = this.propertyService.getPropertiesByOwner(memberId);
          break;

        default:
          console.log('this.session.currentRole()=', this.session.currentRole())
          observableProperties$ = this.propertyService.getPropertiesByClient(clientId);
          break;
      }
      if (!observableProperties$) return;

      observableProperties$.subscribe({
        next: response => {
          this.properties.set(response);
        },
        error: error => console.error('No properties loaded') 
      });
    }

  }



}

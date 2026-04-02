import { Component, inject, Input, input, OnInit, signal } from '@angular/core';
import { PropertyService } from '../../../core/services/property-service';
import { Property } from '../../../types/property';
import { PropertyCard } from '../property-card/property-card';
import { SessionService } from '../../../core/services/session-service';
import { Observable } from 'rxjs';
import { ToastService } from '../../../core/services/toast-service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-property-list',
  imports: [PropertyCard],
  templateUrl: './property-list.html',
  styleUrl: './property-list.css',
})
export class PropertyList implements OnInit {

  protected propertyService = inject(PropertyService);
  private session = inject(SessionService)
  private route =inject(ActivatedRoute);
  protected properties = signal<Property[] | null>([]);
  @Input() memberId?: string;

  ngOnInit(): void {
    const memberId = this.route.parent?.snapshot.paramMap.get('id');
    console.log('ngOnInit PropertyList', memberId)
    this.memberId = memberId ?? undefined; 
    this.loadProperties();
  }



  loadProperties() {
    // admin & property manager view
    const activeClient = this.session.activeClient();

    const clientId = activeClient?.clientId;
    const resolvedMemberId = this.memberId ?? activeClient?.memberId;
     
    var observableProperties$: Observable<Property[]> | undefined;
    if (!!clientId) {
      // If a memberId was passed in, ALWAYS use owner mode
      if (this.memberId) {
        console.log('ngOnInit loadProperties', resolvedMemberId)
        observableProperties$ = this.propertyService.getPropertiesByOwner(resolvedMemberId!);
      } else {
        switch (this.session.currentRole()) {
          case 'admin':
            observableProperties$ = this.propertyService.getPropertiesByClient(clientId);
            break;
          case 'owner':
          case 'board_member':
            if (resolvedMemberId != undefined)
              observableProperties$ = this.propertyService.getPropertiesByOwner(resolvedMemberId);
            break;

          default:
            console.log('this.session.currentRole()=', this.session.currentRole())
            observableProperties$ = this.propertyService.getPropertiesByClient(clientId);
            break;
        }
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

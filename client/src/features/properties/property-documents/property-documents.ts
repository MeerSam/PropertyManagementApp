import { Component, inject, input, OnInit, signal } from '@angular/core';
import { DocumentList } from "../../documents/document-list/document-list"; 
import { SessionService } from '../../../core/services/session-service';
import { ActivatedRoute } from '@angular/router';
import { Property, PropertyOwnership } from '../../../types/property';
import { DocumentRecords } from "../../documents/document-records/document-records";
import { DocumentScope } from '../../../types/document';

@Component({
  selector: 'app-property-documents',
  imports: [   DocumentRecords],
  templateUrl: './property-documents.html',
  styleUrl: './property-documents.css',
})
export class PropertyDocuments implements OnInit {  

  protected session = inject(SessionService) 
  private route = inject(ActivatedRoute);

  // protected property$?: Observable<Property>;
  protected property = signal<Property | null>(null); 
  protected propertyId = signal<string>('');
  protected ownershipId = signal<string>('');
  protected ownerships = signal<PropertyOwnership[]>([]);
  protected scope = signal<DocumentScope>('PropertyHistory');

  
  ngOnInit(): void {
    // here we are using route.parent as details is looking for data from profile
    this.route.parent?.data.subscribe(data => {
      this.property.set(data['property']); 
      this.propertyId.set(this.property()?.id ?? '');

      const propertyOwnerships = this.property()?.ownerships ?? []; 
      if (propertyOwnerships?.length> 0) {  
        this.ownerships.set(propertyOwnerships)
      } 

      // this.ownershipId.set(this.property()?.ownerships?[0]);
    })
  } 

   

}

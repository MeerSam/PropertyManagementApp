import { Component, inject, OnInit, signal } from '@angular/core';
import { SessionService } from '../../../core/services/session-service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../../types/member';
import { MemberService } from '../../../core/services/member-service';
import { Property, PropertyOwnership } from '../../../types/property';
import { DocumentScope } from '../../../types/document';
import { DocumentRecords } from "../../documents/document-records/document-records";

@Component({
  selector: 'app-member-document',
  imports: [  DocumentRecords],
  templateUrl: './member-document.html',
  styleUrl: './member-document.css',
})

export class MemberDocument implements OnInit {
  protected session = inject(SessionService)
  private route = inject(ActivatedRoute);
  protected memberService = inject(MemberService);
  protected scope = signal<DocumentScope>('OwnerTenure');
  // private member = signal<Member | null>(null);  
  // // member signal loaded into the member-service for access to updated member info

  protected ownerships = signal<PropertyOwnership[]>([]);
  protected properties = signal<Property[]>([]);

  tabs = [
    { label: 'Owner Docs', value: 'OwnerTenure' },
    { label: 'Property History', value: 'PropertyHistory' }
  ]
 

  ngOnInit(): void {
    // here we are using route.parent as details is looking for data from profile
    this.route.parent?.data.subscribe(data => {
      this.memberService.member.set(data['member']);
    });  
    const member = this.memberService.member();
    // console.log(member)

    if (member?.propertyOwnerships && member.propertyOwnerships.length > 0){
      this.ownerships.set(member.propertyOwnerships)
    }

    this.properties.set(this.ownerships()
    .filter(o => o.endDate ==null )
    .map(o => o.property ))

     

  }

  setScope(scope: string) { 
    if (scope != this.scope()){
      this.scope.set(scope as DocumentScope ?? null)   ; 
    } 
  }
}
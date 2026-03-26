import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { PropertyService } from '../../../core/services/property-service';
import { SessionService } from '../../../core/services/session-service';
import { Property } from '../../../types/property';
import { ActivatedRoute, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AgePipe } from '../../../core/pipes/age-pipe';

@Component({
  selector: 'app-property-details',
  imports: [DatePipe, AgePipe],
  templateUrl: './property-details.html',
  styleUrl: './property-details.css',
})
export class PropertyDetails implements OnInit {

  protected session = inject(SessionService) 
  private route = inject(ActivatedRoute);

  // protected property$?: Observable<Property>;
  protected property = signal<Property | null>(null); 

  ngOnInit(): void {
    // here we are using route.parent as details is looking for data from profile
    this.route.parent?.data.subscribe(data => {
      this.property.set(data['property']); 
    })
  } 
}

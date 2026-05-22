import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { SessionService } from '../../../core/services/session-service';
import { PropertyService } from '../../../core/services/property-service';
import { Property } from '../../../types/property';
import { Location } from '@angular/common';
import { filter } from 'rxjs';

@Component({
  selector: 'app-property-profile',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './property-profile.html',
  styleUrl: './property-profile.css',
})
export class PropertyProfile implements OnInit {
  protected session = inject(SessionService) 
  private route = inject(ActivatedRoute);
  private router = inject(Router)
  private location = inject(Location);
  protected propertyService = inject(PropertyService);
  protected title = signal<string | undefined>('Profile'); 
  protected owners = computed(() => {
    const p = this.propertyService.property();
    return p ? [...(p.currentOwners ?? [])] : [];
  });

  ngOnInit(): void {
    // coming from the data resolver
    this.route.data.subscribe(data => {
      this.propertyService.property.set(data['property'])  
    })
    this.title.set(this.route.firstChild?.snapshot?.title) // coming from app.route.path_title

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe({
      next: () => {
        this.title.set(this.route.firstChild?.snapshot?.title)
      }
    })
  }

  goBack() {
    this.location.back();
  }

}


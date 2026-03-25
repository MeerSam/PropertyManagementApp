import { Component, inject, input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SessionService } from '../../core/services/session-service';
import { TenantService } from '../../core/services/tenant-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { AppRole } from '../../types/user';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav { 
  protected session = inject(SessionService)
  protected tenantService = inject(TenantService)
  protected router = inject(Router)
  allowedOwnerRoles: AppRole[] = [
    'owner',
    'resident' 
  ];


  titleFromApp = input<string>();
  protected creds: any = {} // empty object  

  login() {
    this.session.login(this.creds).subscribe({
      next: result => {
        this.router.navigateByUrl("/dashboard")
        this.creds = {};
      },
      error: error => alert(error.message),
      complete: () => {
        console.log("completed Login Request from nav")
      }
    });
  }

  logout() {
    this.session.logout();
  }

}

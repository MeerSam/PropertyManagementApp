import { Component, inject, input } from '@angular/core';
import { AccountService } from '../../core/services/account-service';
import { SessionService } from '../../core/services/session-service';
import { TenantService } from '../../core/services/tenant-service';
import { Router, RouterOutlet, RouterLinkWithHref, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastService } from '../../core/services/toast-service';

@Component({
  selector: 'app-nav-sidebar',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './nav-sidebar.html',
  styleUrl: './nav-sidebar.css',
})
export class NavSidebar {
  protected accountService = inject(AccountService)
  protected sessionService = inject(SessionService)
  protected tenantService = inject(TenantService)
  private toastService =inject(ToastService)
  protected router = inject(Router)
  // titleFromApp = input<string>();
  protected creds: any = {} // empty object  

  login() {
    this.sessionService.loginAndSelectClient(this.creds).subscribe({
      next: result => {
        this.router.navigateByUrl("/members")
        this.creds = {};
      },
      error: error => {
        this.toastService.error(error.error)
      },
      complete: () => {
        console.log("completed Login Request from nav")
      }
    });
  }

  logout() {
    this.accountService.logout();
  }
}

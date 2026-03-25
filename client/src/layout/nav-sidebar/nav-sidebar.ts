import { Component, inject } from '@angular/core';
import { SessionService } from '../../core/services/session-service';
import { Router, RouterOutlet, RouterLink } from '@angular/router';
import { ToastService } from '../../core/services/toast-service';

@Component({
  selector: 'app-nav-sidebar',
  imports: [RouterOutlet, RouterLink],
  templateUrl: './nav-sidebar.html',
  styleUrl: './nav-sidebar.css',
})
export class NavSidebar { 
  protected session = inject(SessionService) 
  private toastService =inject(ToastService)
  protected router = inject(Router)
  // titleFromApp = input<string>();
  protected creds: any = {} // empty object  

  login() {
    this.session.login(this.creds).subscribe({
      next: result => {
        this.router.navigateByUrl("/dashboard")
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
    this.session.logout();
  }
}

import { Component, inject, signal } from '@angular/core';
import { SessionService } from '../../core/services/session-service';
import { Router, RouterOutlet, RouterLink, RouterLinkActive, RouterModule } from '@angular/router';
import { ToastService } from '../../core/services/toast-service';
import { themes } from '../theme';

@Component({
  selector: 'app-nav-sidebar',
  imports: [RouterOutlet, RouterLink, RouterModule],
  templateUrl: './nav-sidebar.html',
  styleUrl: './nav-sidebar.css',
})
export class NavSidebar {
  protected session = inject(SessionService)
  private toastService = inject(ToastService)
  protected router = inject(Router)
  // titleFromApp = input<string>();
  protected creds: any = {} // empty object  
  protected selectedTheme = signal<string>(localStorage.getItem('theme') || 'light');
  protected themes = themes;

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

  handleSelectTheme(theme: string) {
    this.selectedTheme.set(theme);
    localStorage.setItem('theme', theme);
    document.documentElement.setAttribute('data-theme', theme);
    // closing dropdown after the theme selected
    const elem = document.activeElement as HTMLDivElement;
    if (elem) elem.blur();
  }
}

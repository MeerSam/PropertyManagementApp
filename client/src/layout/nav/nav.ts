import { Component, inject, input, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { SessionService } from '../../core/services/session-service';
import { TenantService } from '../../core/services/tenant-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  protected accountService = inject(AccountService)
  protected sessionService = inject(SessionService)
  protected tenantService = inject(TenantService)
  protected router = inject(Router)

  titleFromApp = input<string>();
  protected creds: any = {} // empty object  

  login() { 
    this.sessionService.loginAndSelectClient(this.creds).subscribe({
      next: result => { 
        this.router.navigateByUrl("/members")
        this.creds = {};  
      },
      error: error => alert(error.message),
      complete: ()=> {
        console.log("completed Login Request from nav")  
      } 
    }); 
  }

  logout(){ 
    this.accountService.logout();
  }
  
}

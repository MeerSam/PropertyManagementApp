import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ApiError } from '../../../types/error';

@Component({
  selector: 'app-server-error',
  imports: [],
  templateUrl: './server-error.html',
  styleUrl: './server-error.css',
})
export class ServerError {
  private router = inject(Router);
  protected error : ApiError;
  protected showDetails = false
  constructor() {
    // we get access to the navigationExtras that we passed as state only inside the constructor from the error interceptor
    const navigation = this.router.currentNavigation();
    this.error = navigation?.extras?.state?.['error'];
  }

  detailsToggle(){
    this.showDetails = !this.showDetails;
  }

}

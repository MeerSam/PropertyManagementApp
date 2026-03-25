import { inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { SessionService } from './session-service';

@Injectable({
  providedIn: 'root',
})
export class InitService { 
  protected session = inject(SessionService)  

  init(): Observable<null> {  
    this.session.initSession();
    // moved all the code to sessionService for simplicity and clean code
    return of(null) //Observable<null>
  }
}

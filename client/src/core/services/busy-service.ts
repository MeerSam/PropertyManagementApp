import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BusyService { 

  busyRequestCount = signal(0); // initial value 0

  busy(){
    // console.log('Busy service')
    this.busyRequestCount.update(current => current + 1);
    // console.log(`Busy Count ${this.busyRequestCount}`)
  }

  idle() {
    // console.log('Idle service')
    this.busyRequestCount.update(current => Math.max(0, current -1));
    // console.log(`Busy Count ${this.busyRequestCount}`)

  }
  
}

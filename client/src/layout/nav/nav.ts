import { Component, input } from '@angular/core';

@Component({
  selector: 'app-nav',
  imports: [],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  TitleFromApp = input<string>();
  creds: any={}

  login(){
    console.log(this.creds);
  }

}

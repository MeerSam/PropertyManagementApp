import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { NavSidebar } from "../layout/nav-sidebar/nav-sidebar";
import { Nav } from "../layout/nav/nav";
 

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavSidebar],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App  { 
  protected router = inject(Router);   
}

import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Nav } from "../layout/nav/nav"; 
import { environment } from '../environments/environment.development';

@Component({
  selector: 'app-root',
  imports: [Nav],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  
  private http = inject(HttpClient);
   private baseUrl = environment.apiUrl;
  protected readonly title = signal('Property Management');
  protected members= signal<any>([]);
  id: string = 'bob-id';

  ngOnInit(): void {
    this.http.get('members').subscribe({
      next: response => this.members.set(response),
      error: error => console.log(error),
      complete: ()=> console.log("Request Completed")
    })

    this.http.get('https://localhost:5001/api/members/clients/'+this.id).subscribe({
      next: response => this.members.set(response),
      error: error => console.log(error),
      complete: ()=> console.log("Request Completed")
    })
  }
   
}

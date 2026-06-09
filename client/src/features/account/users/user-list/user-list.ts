import { Component, inject, OnInit, signal } from '@angular/core';
import { MemberCard } from "../../../members/member-card/member-card";
import { SessionService } from '../../../../core/services/session-service';
import { User } from '../../../../types/user';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-list',
  imports: [RouterLink],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css',
})
export class UserList implements OnInit {
 protected session = inject(SessionService) 

  protected users =  signal<User[]>([]) ;

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() { 
    this.session.loadUsers().subscribe({
      next: result => {
        result.filter(u => u.isMemberLinked == false)
        this.users.set(result);
      }
    });

  }

}

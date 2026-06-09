import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { EditableUser, User } from '../../../../types/user';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { SessionService } from '../../../../core/services/session-service';
import { ToastService } from '../../../../core/services/toast-service';
import { filter } from 'rxjs';

@Component({
  selector: 'app-user-details',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './user-details.html',
  styleUrl: './user-details.css',
})
export class UserDetails implements OnInit {

  protected route = inject(ActivatedRoute)
  protected session = inject(SessionService)
  protected toast = inject(ToastService)
  private router = inject(Router);
  protected userId = signal<string | null | undefined>("");
  protected user = signal<User | null>(null);


  protected title = signal<string | undefined>('Profile');
  protected isCurrentUser = computed(() => {
    return this.session.currentUser()?.id === this.userId();
  })

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.userId.set(id);

    // coming from the data resolver
    this.route.data.subscribe(data => {
      this.user.set(data['user']);
    });

    this.title.set(this.route.firstChild?.snapshot?.title)
    // getting the Page Title from app.routes
    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe({
      next: () => {
        this.title.set(this.route.firstChild?.snapshot.title)
      }
    })


  }
}

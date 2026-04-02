import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Member } from '../../../types/member';
import { ActivatedRoute, Router, RouterOutlet, RouterLinkWithHref, RouterLink, RouterModule } from '@angular/router';
import { SessionService } from '../../../core/services/session-service';
import { MemberService } from '../../../core/services/member-service';

@Component({
  selector: 'app-member-detailed',
  imports: [RouterOutlet, RouterLink, RouterModule],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css',
})
export class MemberDetailed implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private session = inject(SessionService);
  protected memberService = inject(MemberService);
  // member = signal<Member | null>(null)  ; // member signal loaded into the member-service for access to updated member info
  loading = true;
  protected title = signal<string | undefined>('Profile');
  protected isCurrentUser = computed(() => {
    return this.session.currentUser()?.id === this.memberService.member()?.userId;
  })


  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    // coming from the data resolver
    this.route.data.subscribe(data => {
      this.memberService.member.set(data['member']);
      const child = this.route.firstChild;

      if(!child) return ;
      child.data.subscribe( data =>{        
        console.log('OnInit MemberDetailed data', data['member'])
      }) 
    })
    this.route.url.subscribe(() => {
      const child = this.route.firstChild;
      if (child) {
        this.title.set(child.snapshot.title);
      }
    });

    // this.title.set(this.route.firstChild?.snapshot?.title) // coming from app.route.path_title

  }

}

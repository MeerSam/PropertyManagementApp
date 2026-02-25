import { Component, inject, OnInit, signal } from '@angular/core';
import { AccountService } from '../../../core/services/account-service';
import { MemberService } from '../../../core/services/member-service';
import { Member, MemberParams } from '../../../types/member';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-member-list',
  imports: [],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css',
})
export class MemberList implements OnInit{
  protected memberService = inject(MemberService)
  protected memberParams = new MemberParams();

  protected members =  signal<Member[]>([]) ;

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() { 
    this.memberService.getMembers(this.memberParams).subscribe({
      next: result => {
        this.members.set(result);
      }
    });

  }

}

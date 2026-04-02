import { Component, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { Member, MemberParams } from '../../../types/member';
import { MemberCard } from "../member-card/member-card";

@Component({
  selector: 'app-member-list',
  imports: [MemberCard],
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

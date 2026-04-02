import { Component, HostListener, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { EditableMember, Member } from '../../../types/member';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MemberService } from '../../../core/services/member-service';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastService } from '../../../core/services/toast-service';
import { SessionService } from '../../../core/services/session-service';
import { AgePipe } from '../../../core/pipes/age-pipe';

@Component({
  selector: 'app-member-profile',
  imports: [DatePipe, FormsModule, AgePipe],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css',
})
export class MemberProfile implements OnInit, OnDestroy {

  @ViewChild('editForm') editForm?: NgForm // getting access to the TemplateForm in comeponent using viewChild
  // prevent completely moving away on any window of the browser not just angular compoent. 
  // prevent guard does not have access to that so we must use this
  @HostListener('window:beforeunload', ['$event']) notify($event: BeforeUnloadEvent): void {
    if (this.editForm?.dirty) {
      $event.preventDefault();
    }
  }

  protected route = inject(ActivatedRoute)
  protected memberService = inject(MemberService)
  private session = inject(SessionService)
  protected toast = inject(ToastService);
  // // moving member signal this to member-service to allow updates to flow all areas of the app when a profile gets updated
  // protected member = signal<Member | undefined>(undefined); 
  protected editabelMember: EditableMember = {
    firstName: '',
    lastName: '',
    email: '',
    description: '',
    displayName: '',
  };
  ngOnInit(): void {
    console.log('OnInit MemberProfile memServ before', this.memberService.member());
    this.route.parent?.data.subscribe(data => { 
        if (data) {
          this.memberService.member.set(data['member'])
          this.editabelMember = {
            description: this.memberService.member()?.description || '',
            firstName: this.memberService.member()?.firstName || '',
            lastName: this.memberService.member()?.lastName || '',
            email: this.memberService.member()?.email || '',
            displayName: this.memberService.member()?.displayName || ''
          }
        } else {
          this.toast.error('Error loading ember from parent')
        }
      });
    console.log('OnInit MemberProfile memServ after', this.memberService.member());
    console.log('OnInit MemberProfile editableMember', this.editabelMember);
  }

  ngOnDestroy(): void {
    if (this.memberService.editMode()) {
      this.memberService.editMode.set(false);

    }
  }

  updateProfile() {
    if (!this.memberService.member()) return;
    const updatedMember = { ...this.memberService.member(), ...this.editabelMember }

    this.memberService.updateMember(updatedMember).subscribe({
      next: () => {
        this.toast.success('Profile Updated for member succesfully');
        this.memberService.editMode.set(false);
        this.memberService.member.set(updatedMember as Member);
        if (updatedMember.displayName !== this.session.currentUser()?.displayName) {
          this.session.currentUser.update(u => ({ ...u!, displayName: updatedMember.displayName }));
        }
      },
      error: error => this.toast.error('Error while saving.' + error)
    });

  }

}

import { inject } from '@angular/core';
import { ResolveFn, Router } from '@angular/router';
import { MemberService } from '../../core/services/member-service';
import { EMPTY } from 'rxjs';
import { Member } from '../../types/member';

export const memberResolver: ResolveFn<Member> = (route, state) => {
  const memberService = inject(MemberService);
  const memberId = route.paramMap.get('id');
  const router = inject(Router);
  if (!memberId) {
    router.navigateByUrl('/not-found');
    return EMPTY;
  }
  console.log('at resolver', memberId)
  return memberService.getMember(memberId);
};

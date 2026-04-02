import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { EditableMember, Member, MemberParams } from '../../types/member';
import { SessionService } from './session-service';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
   //services are singletons they are instantiated when the angular app starts
   //only one instance exists for the entire lifetime of the application.

  private http = inject(HttpClient);
  private session = inject(SessionService);  
  private baseUrl = environment.apiUrl;
  editMode = signal(false);
  member =signal<Member|null>(null);


  getMembers(memberParams: MemberParams) {
    const clientId = this.session.activeClient()?.clientId;
    let params  = new HttpParams(); // HttpParams: this will allow us to send something up as query string parameters to our API.
    params =  params.append('pageNumber', memberParams.pageNumber);
    params =  params.append('pageSize', memberParams.pageSize); 
    params =  params.append('orderBy', memberParams.orderBy); 

    return this.http.get<Member[]>(this.baseUrl+ 'members')    
  }
  getMember(id: string) {
    // here we're loading member and making use of the side effect to load member data 
    //to resolver since we could not update the details of member in all points without refreshing data.
    return this.http.get<Member>(this.baseUrl + 'members/' + id).pipe(
      tap(member =>{
        this.member.set(member)
      })
    ); 
  }

  updateMember(data: EditableMember ){
    return this.http.put(this.baseUrl + 'members', data);
  }
 
}

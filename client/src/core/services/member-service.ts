import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member, MemberParams } from '../../types/member';
import { SessionService } from './session-service';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
   //services are singletons they are instantiated when the angular app starts
   //only one instance exists for the entire lifetime of the application.

  private http = inject(HttpClient);
  private session = inject(SessionService);  
  private baseUrl = environment.apiUrl;

  getMembers(memberParams: MemberParams) {
    const clientId = this.session.activeClient()?.clientId;
    let params  = new HttpParams(); // HttpParams: this will allow us to send something up as query string parameters to our API.
    params =  params.append('pageNumber', memberParams.pageNumber);
    params =  params.append('pageSize', memberParams.pageSize); 
    params =  params.append('orderBy', memberParams.orderBy); 

    return this.http.get<Member[]>(this.baseUrl+ '/members', {params})    
  }
  getMember(id: string) {
    return this.http.get<Member>(this.baseUrl + 'members/' + id); 
  }
 
}

import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs';
import { AuthErrorResponse, AuthSuccessResponse, ClientSelectLoginResponse, RegisterDto, User, UserDto } from '../../types/user';
import { Client } from '../../types/client';


@Injectable({
  providedIn: 'root',
})
export class AccountService {
  //services are singletons they are instantiated when the angular app starts :only one instance exists for the entire lifetime of the application.
  //- It lives forever: stateless services


  //@Injectable decorator allow this class to be used as dependency injection
  // in our components
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl; //'https://localhost:5001/api/';
  currentUser = signal<User | null>(null);


  register(creds: RegisterDto) {
    return this.http.post<AuthSuccessResponse | ClientSelectLoginResponse | AuthErrorResponse>(this.baseUrl + 'account/login', creds);
  }


  login(creds: any) {
    console.log(creds.email);
    return this.http.post<AuthSuccessResponse | ClientSelectLoginResponse | AuthErrorResponse>(this.baseUrl + 'account/login', creds);
  }

  setClientUserAfterLogin(response: AuthSuccessResponse) {
    localStorage.setItem('user', JSON.stringify(response.user));
    const _user = this.mapUserDtoToUser(response.user, response.accessToken);
    if (_user) {
      this.setCurrentUser(_user);
      // client info from the reponse if only one client comes back as clientSelected. if two or more 
      var client = response.user.activeClient;
      client.isActiveClient = true;
      localStorage.setItem('activeClient', JSON.stringify(client));
      localStorage.removeItem('selectionToken');
    }
    else {
      this.logout()
    }
  }

  setCurrentUser(user: User) {
    this.currentUser.set(user);
  }

  // A type guard in TypeScript is a small function that checks the shape of an object at runtime and tells TypeScript,
  //  “Inside this block, you can safely treat this as a specific type.”

  isAuthSuccess(response: any): response is AuthSuccessResponse {
    return response && 'accessToken' in response && 'refreshToken' in response;
  }

  isClientSelect(response: any): response is ClientSelectLoginResponse {
    return response && 'selectionToken' in response && 'availableClients' in response;
  }

  isAuthError(response: any): response is AuthErrorResponse {
    return response && 'message' in response && !('accessToken' in response);
  }

  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('activeClient');
    this.currentUser.set(null);

  }

  getAvailableClients(): string[] {
    const raw = localStorage.getItem('availableClients');

    if (!raw) return [];
    try {
      return JSON.parse(raw) as string[];
    } catch {
      return [];
    }
  }

  private mapUserDtoToUser(dto: UserDto, accessToken: string): User {
    return {
      ...dto,
      accessToken,
      // Only include this if your User type still has appRole
      appRole: (dto as any).appRole
    };
  }


  private getRolesFromToken(user: User): string[] {
    // passsing user as parameter and returning string array
    const payload = user.accessToken.split(".")[1]; //get payload
    // token has 3 parts : 1st part : token header (info about expiration and type of token)
    //part 2 : payload : encoded not encrypted 
    //part 3: encrypted and cannot decipher withou the secret which is in API and never leaves server

    // payload : decode // base64 encoded string
    const decoded = atob(payload); //we'll use a native JavaScript function called atob 

    const jsonPayload = JSON.parse(decoded);

    return Array.isArray(jsonPayload.role) ? jsonPayload.role : [jsonPayload.role]
  }


}

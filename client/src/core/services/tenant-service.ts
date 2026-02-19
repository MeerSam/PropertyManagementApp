import { inject, Injectable, signal } from '@angular/core';
import { Client } from '../../types/client';
import { HttpClient } from '@angular/common/http';
import { AuthErrorResponse, AuthSuccessResponse, SelectClientDto } from '../../types/user';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TenantService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl; //'https://localhost:5001/api/'; 

  private storedActiveClient = localStorage.getItem('activeClient');
  private storedSelectionToken = localStorage.getItem('selectionToken');
  private storedAvailableClient = localStorage.getItem('availableClients');

  activeClient = signal<Client | null>(this.storedActiveClient ? JSON.parse(this.storedActiveClient) as Client : null);
  selectionToken = signal<string>(this.storedSelectionToken ? JSON.parse(this.storedSelectionToken) : '');
  availableClients = signal<Client[]>(this.storedAvailableClient ? JSON.parse(this.storedAvailableClient) : []);


  selectClient(creds: SelectClientDto) {
    return this.http.post<AuthSuccessResponse | AuthErrorResponse>(this.baseUrl + 'account/selectclient', creds);
  }

  setActiveClient(client: Client | null) {
    if(!client) alert('Client Missing');
    this.activeClient.set(client);
    localStorage.setItem('activeClient', JSON.stringify(client));

  }
  setSelectionToken(selectionToken: string) {
    localStorage.setItem('selectionToken', selectionToken);
    this.selectionToken.set(selectionToken);
  }

  setAvailableClient(clients: Client[]) {
    this.availableClients.set(clients);
    localStorage.setItem('availableClients', JSON.stringify(clients));
  }

  getActiveClient(): Client | null {
    return this.activeClient();
  }

  clear() { 
    this.clearActiveClient();
    localStorage.removeItem('selectionToken');
    this.selectionToken.set('')

  }

  clearActiveClient() {
    this.activeClient.set(null);
    localStorage.removeItem('activeClient');
  }
}

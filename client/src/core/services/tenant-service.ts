import { inject, Injectable, signal } from '@angular/core';
import { Client } from '../../types/client';
import { HttpClient } from '@angular/common/http';
import { AuthErrorResponse, AuthSuccessResponse, SelectClientDto } from '../../types/auth';
import { environment } from '../../environments/environment';
import { ToastService } from './toast-service';

@Injectable({
  providedIn: 'root',
})
export class TenantService {
  //services are singletons they are instantiated when the angular app starts
  private http = inject(HttpClient);
  private toastService = inject(ToastService)
  baseUrl = environment.apiUrl; //'https://localhost:5001/api/'; 

  // ─── State (self-hydrating from localStorage) ─────────────────
  // Signals initialize from localStorage on app start automatically.
  // SessionService.initSession() does NOT need to set these manually.
  activeClient = signal<Client | null>(this.loadFromStorage('activeClient'));
  selectionToken = signal<string>(this.loadFromStorage('selectionToken') ?? '');
  availableClients = signal<Client[]>(this.loadFromStorage('availableClients') ?? []);

  initClientFromStorage(): void {
  // activeClient, selectionToken, availableClients signals
  // already hydrate from localStorage in signal declarations —
  // so init is already done! No extra method needed.
  // Just expose a check:
  if(!this.activeClient) this.toastService.error('Active client is missing during initializing');
}

  // ─── HTTP Calls (pure — no side effects) ──────────────────────
  // POST /api/account/selectclient
  selectClient(creds: SelectClientDto) {
    return this.http.post<AuthSuccessResponse | AuthErrorResponse>(this.baseUrl + 'account/selectclient', creds);
  }
  // ─── State Mutations (called only by SessionService) ──────────
  setClientFromResponse(response: AuthSuccessResponse) {
    var client = response.user.activeClient;
    if (!client) {
      this.toastService.warning('Active client is missing from response');
      return;
    }
    if (!!client) { 
      this.setActiveClient({ ...client, isActiveClient: true });  
      localStorage.removeItem('selectionToken');
    } 
    this.clearSelectionToken(); // selection flow is complete
  }

  setActiveClient(client: Client | null) {
    if (!client) this.toastService.warning('Active client is missing');
    this.activeClient.set(client);
    if (!!client) {
      localStorage.setItem('activeClient', JSON.stringify(client));
    }
    else {
      localStorage.removeItem('activeClient');
    }

  }

  setSelectionToken(selectionToken: string) {
    if (!!selectionToken) {
      this.selectionToken.set(selectionToken);
      localStorage.setItem('selectionToken', selectionToken);
    }
    else {
      this.selectionToken.set('');
      localStorage.removeItem('selectionToken');
    }
  }

  setAvailableClients(clients: Client[]) {
    this.availableClients.set(clients);
    if (clients.length > 0) {
      localStorage.setItem('availableClients', JSON.stringify(clients));
    } else {
      localStorage.removeItem('availableClients');
    }
  }

  getActiveClient(): Client | null {
    return this.activeClient();
  }

  clearActiveClient() {
    this.activeClient.set(null);
    this.setSelectionToken('')
    localStorage.removeItem('activeClient');
    localStorage.removeItem('selectionToken');
  }
    /**
   * Clears all tenant state. Called by SessionService.logout().
   */
  clear(): void {
    this.activeClient.set(null);
    this.selectionToken.set('');
    this.availableClients.set([]);
    localStorage.removeItem('activeClient');
    localStorage.removeItem('selectionToken');
    localStorage.removeItem('availableClients');
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

  private loadFromStorage<T>(key: string): T | null {
    try {
      const raw = localStorage.getItem(key);
      return raw ? JSON.parse(raw) as T : null;
    } catch {
      return null;
    }
  }
  // ─── Private Helpers ──────────────────────────────────────────
  private clearSelectionToken(): void {
    this.selectionToken.set('');
    localStorage.removeItem('selectionToken');
  }


}

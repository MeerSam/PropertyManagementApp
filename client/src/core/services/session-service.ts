import { computed, inject, Injectable, signal } from '@angular/core';
import { AccountService } from './account-service';
import { TenantService } from './tenant-service';
import { Client } from '../../types/client';
import { LoginCreds, LoginOutcome, SelectClientDto } from '../../types/auth';
import { map, catchError, of, Observable } from 'rxjs';
import { AppRole, UserClientAccessInfo } from '../../types/user';

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  //services are singletons they are instantiated when the angular app starts
  private accountService = inject(AccountService);
  private tenantService = inject(TenantService);

  // ─── Expose unified signals (components read from here only) ───
  readonly currentUser = this.accountService.currentUser;      // Signal<User | null>
  readonly activeClient = this.tenantService.activeClient;      // Signal<Client | null>
  readonly availableClients = this.tenantService.availableClients;// Signal<Client[]>
  readonly selectionToken = this.tenantService.selectionToken;    // Signal<string>


  // Computed: is the session fully ready (user + client both present)?
  readonly isSessionReady = computed(() =>
    !!this.currentUser() && !!this.activeClient()
  );

  // Computed: current role — single source of truth
  readonly currentRole = computed(() =>
    this.currentUser()?.appRole ?? null
  );

  // Computed: needs client selection (multi-client user at login)
  readonly needsClientSelection = computed(() =>
    !this.activeClient() && !!this.selectionToken() && this.availableClients().length > 1
  );
  // ─── Called once in app.config.ts via APP_INITIALIZER ────────
  initSession(): Observable<null> {
    // Step 1: restore user
    const userRestored = this.accountService.initUserFromStorage();
    if (!userRestored) return of(null); // no user = nothing to restore

    // Step 2: TenantService signals already self-hydrate from localStorage
    // Just validate the restored state is consistent
    const client = this.activeClient();
    const token = this.selectionToken();
    const hasNeither = !client && !token;

    if (hasNeither) {
      // User exists but no client context — force clean state
      console.warn('InitSession: user found but no client context, clearing session');
      this.logout();
    }
    return of(null);
  }

  login(creds: LoginCreds): Observable<LoginOutcome> {
    console.log('In session: login');
    return this.accountService.login(creds).pipe(
      map(response => {
        if (this.accountService.isAuthSuccess(response)) {
          // successfully logged in :only one client 
          this.accountService.setCurrentUserFromResponse(response);
          this.tenantService.setClientFromResponse(response);
          return { status: 'success' } as LoginOutcome;
        }
        if (this.accountService.isClientSelect(response)) {
          // Needs User Confirmation on which client they want to choose for this session.
          console.log()
          this.tenantService.setSelectionToken(JSON.stringify(response.selectionToken));
          const _availableClients = this.mapToClient(response.availableClients)
          this.tenantService.setAvailableClients(_availableClients);
          localStorage.setItem('availableClients', JSON.stringify(_availableClients));
          return { status: 'needs_client_selection', clients: response.availableClients } as LoginOutcome;
        }
        if (this.accountService.isAuthError(response)) {
          console.error(response.message);
          return { status: 'error', message: response.message } as LoginOutcome;
        }
        return { status: 'error', message: 'Unknown error during login' } as LoginOutcome;
      }),
      catchError(err => of({
        status: 'error',
        message: err?.error?.message ?? 'Server error'
      } as LoginOutcome))
    );
  }

  // ─── Step 2: Client Selection ─────────────────────────────────
  selectClient(selectedClient: Client): Observable<LoginOutcome> {
    if ((!this.tenantService.selectionToken())) {
      console.error('Cannot complete request. Client selection token is missing');
      of({
        status: 'error',
        message: 'Cannot complete request. Client selection token is missing'
      } as LoginOutcome)
    }
    const creds: SelectClientDto = {
      clientId: selectedClient.clientId,
      selectionToken: this.tenantService.selectionToken()
    };

    return this.tenantService.selectClient(creds).pipe(
      map(response => {
        if (this.accountService.isAuthSuccess(response)) {
          this.accountService.setCurrentUserFromResponse(response);
          this.tenantService.setClientFromResponse(response);
          return { status: 'success' } as LoginOutcome;
        }
        if (this.accountService.isAuthError(response)) {
          console.error(response.message);
          return { status: 'error', message: response.message } as LoginOutcome;
        }
        return { status: 'error', message: 'Unknown error during login' } as LoginOutcome;
      })
    )
  }

  // ─── Role Helpers ─────────────────────────────────────────────
  hasRole(...roles: AppRole[]): boolean {
    return this.accountService.hasRole(...roles);
  }

  // ─── Logout ───────────────────────────────────────────────────
  logout() {
    this.tenantService.clearActiveClient();
    this.accountService.logout();
  }

  // ─── Switch Client (without full re-login) ────────────────────
  switchClient() {
    this.tenantService.clearActiveClient();
    // Keep user, clear client — router navigates to /select-client
  }

  private mapToClient(accessInfo: UserClientAccessInfo[]): Client[] {
    const clients: Client[] = accessInfo.map(x => ({
      clientId: x.clientId,
      clientName: x.clientName,
      isActiveClient: false,        // you will set this later
      userId: x.userId,
      memberId: x.memberId ?? undefined
    }));
    return clients;
  }






}

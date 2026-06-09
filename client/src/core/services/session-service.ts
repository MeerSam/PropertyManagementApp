import { computed, inject, Injectable, signal } from '@angular/core';
import { AccountService } from './account-service';
import { TenantService } from './tenant-service';
import { Client } from '../../types/client';
import { ForgotPasswordDto, LoginCreds, LoginOutcome, RegisterDto, RegisterResponse, SelectClientDto, UserCredsChange } from '../../types/auth';
import { map, catchError, of, Observable } from 'rxjs';
import { Role, EditableUser, User, UserClientAccessInfo, APP_ROLE_LABELS, APP_ROLE } from '../../types/user';
import { SelectOption } from '../../types/select';

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
  private adminRoles = ['admin', 'board_member', 'property_manager'];

  // Computed: is the session fully ready (user + client both present)?
  readonly isSessionReady = computed(() =>
    !!this.currentUser() && !!this.activeClient()
  );

  // Computed: current role — single source of truth
  readonly currentRole = computed(() =>
    this.currentUser()?.role ?? null
  );

  readonly isAdminRole = computed<boolean>(() =>
    this.adminRoles.includes(this.currentRole() ?? '')
  )

  // Computed: needs client selection (multi-client user at login)
  readonly needsClientSelection = computed(() =>
    !this.activeClient() && !!this.selectionToken() && this.availableClients().length > 1
  );

  readonly roleOptions = computed<SelectOption[]>(() => {
    const currentUserRole = this.currentUser()?.role;
    if (currentUserRole == 'admin') {
      return APP_ROLE
        .map(r => ({ label: APP_ROLE_LABELS[r], value: r }))
    } else if (currentUserRole == 'property_manager') {
      return [
        { label: 'Owner', value: 'owner' },
        { label: 'Resident', value: 'resident' },
        { label: 'Board Member', value: 'board_member' },
        { label: 'Property Manager', value: 'property_manager' }];
    }
    return [
      { label: 'Owner', value: 'owner' },
      { label: 'Resident', value: 'resident' }];
  })


  editMode = signal(false);

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
  register(creds: RegisterDto) {
    return this.accountService.register(creds);
  }

  login(creds: LoginCreds): Observable<LoginOutcome> {
    // console.log('In session: login');
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

  setCurrentUser(user: User) {
    this.accountService.setCurrentUser(user);
  }

  updateUser(data: EditableUser) {
    return this.accountService.updateUser(data);
  }

  loadUserForEdit(userId: string) {
    return this.accountService.loadUserById(userId);
  }

  loadUsers() {
    return this.accountService.getUsers();
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
  hasRole(...roles: Role[]): boolean {
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

  updateCredentials(credentials: UserCredsChange) {
    return this.accountService.updateUserCreds(credentials);
  }

  forgotPassword(data: ForgotPasswordDto) {
    return this.accountService.forgotPassword(data);
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

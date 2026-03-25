import { Client } from "./client";
import { UserClientAccessInfo, UserDto } from "./user";

export type LoginCreds = {
    email: string;
    password: string;
}

export interface SelectClientDto  {
    clientId: string;
    selectionToken: string;
}


export interface AuthSuccessResponse {
    success: boolean;
    accessToken: string;
    refreshToken: string;
    clientName: string;
    displayName: string;
    expiresAt: string;
    user: UserDto;

}
export interface ClientSelectLoginResponse {
    userId: string;
    displayName: string;
    email: string;
    message: string;
    selectionToken: string;
    availableClients: UserClientAccessInfo[];
}


export interface RegisterDto {
  clientId: string;
  email: string;
  password: string;
  displayName: string;
  firstName: string;
  lastName: string;
  isBoardMember: boolean;
  isAdminMember: boolean;
  dateOfBirth: string;
  gender: string;
  appRole: string;
}

export type AuthErrorResponse = {
     message: string;
     error: string[];
}


export type LoginOutcome =
  | { status: 'success' }
  | { status: 'needs_client_selection'; clients: UserClientAccessInfo[] }
  | { status: 'error'; message: string };
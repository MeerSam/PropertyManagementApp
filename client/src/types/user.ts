import { Client } from "./client";
import { Member } from "./member";


export type User = {
  id: string;
  email: string;
  displayName: string;
  firstName: string;
  lastName: string;
  imageUrl?: string;
  appRole: AppRole;
  activeClient: Client;
  accessToken: string;
}

export type UserDto = {
  id: string;
  email: string;
  displayName: string;
  firstName: string;
  lastName: string;
  imageUrl?: string;
  appRole: AppRole;
  activeClient: Client;
  availableClients: UserClientAccessInfo[];
}

export type UserClientAccessInfo = {
  userId: string;
  displayName: string;
  email: string;
  clientId: string;
  clientName: string;
  memberId: string;
  hasMemberProfile: boolean;
  imageUrl?: string
  member?: Member
}


export type AppRole =
  | 'admin'
  | 'board_member'
  | 'property_manager'
  | 'owner'
  | 'resident';


export type DocumentScope =
  | 'Public'
  | 'Community'         // All active members of this Client
  | 'PropertyHistory'   // Current Primary owner + board + managers
  | 'OwnerTenure'       // Only that specific tenure's Primary owner + managers


export const APP_ROLE: AppRole[] = [
  'admin',
  'board_member',
  'property_manager',
  'owner',
  'resident'
] as const;

export const APP_ROLE_LABELS: Record<AppRole, string> = {
  admin: 'Administrator',
  board_member: 'Board Member',
  property_manager: 'Property Manager',
  owner: 'Owner',
  resident: 'Resident (household member or tenant)'

};



export type EditableUser = {
  userId: string;
  firstName?: string;
  lastName?: string;
  displayName?: string;
  description?: string;
  email?: string
  imageUrl?: string
}

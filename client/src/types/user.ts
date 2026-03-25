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
    availableClients:UserClientAccessInfo[];
}

export type UserClientAccessInfo = {
    userId: string;
    displayName: string;
    email: string;
    clientId: string;
    clientName: string;
    memberId: string;
    hasMemberProfile: boolean;
    imageUrl?:  string
    member?: Member
}


export type AppRole = 
  | 'admin'
  | 'board_member'
  | 'property_manager'
  | 'owner'
  | 'resident';



 
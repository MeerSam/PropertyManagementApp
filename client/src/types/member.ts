export type Member = {
  id: string;
  email: string
  firstName: string;
  lastName: string;
  displayName: string;
  dateOfBirth: string;
  gender: string;
  imageUrl?: string;
  created: string;
  lastActive: string;
  description?: string;
  clientId: string;
  userId: string;
  role?: string;
}


export class MemberParams {
  gender?: string;
  pageNumber = 1;
  pageSize = 10;
  orderBy = 'lastActive';
}


export type MemberRole =
  | 'primary'
  | 'secondary'
  | 'readonly'
  | 'restricted'


export type EditableMember = {
  firstName: string;
  lastName: string;
  displayName: string;
  description?: string;
  email: string

}

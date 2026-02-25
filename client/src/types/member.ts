export type Member = {
    id: string;
    firstName: string;
    lastName: string;
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
  pageNumber =1;
  pageSize =10;
  orderBy = 'lastActive';
}

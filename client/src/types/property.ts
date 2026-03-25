import { Member } from "./member"

export type Property = {
    id: string
    address: string
    unit: any
    city: string
    state: string
    zipCode: string
    lotNumber: string
    createdAt: string
    squareFeet: number
    bedrooms: number
    bathrooms: number
    isRented: boolean
    clientId: string
    ownerships: PropertyOwnership[]
    currentOwners? : Member[]
    mailAddress: string
    mailCity: string
    mailState: string
    mailZip: string
    mailCountry:string
}

export type PropertyOwnership = { 
    id: string
    propertyId: string
    memberId : string
    startDate: string
    endDate: string
    ownedEndDate: string
    ownershipType: OwnershipType
    ownershipPercentage:string
    member:Member
}

export enum OwnershipType {
  Primary = 0,
  CoOwner = 1
}



import { Member } from "./member"

export type Property = {
    id: string
    address: string
    unit: string
    city: string
    state: string
    zipCode: string
    country: string
    lotNumber: string
    createdAt: string
    assignedParking: string   
    lastUpdated: string
    lastUpdatedBy: string
    squareFeet: number; 
    bedrooms: number
    bathrooms: number
    isRented: boolean
    isSameAddress: boolean
    mailAddress: string
    mailUnit: string
    mailCity: string
    mailState: string
    mailZipCode: string
    mailCountry: string
    clientId: string
    ownerships: PropertyOwnership[]  
    currentOwners?: Member[]
}

export type PropertyOwnership = {
    id: string
    propertyId: string
    memberId: string
    startDate: string
    endDate: string
    ownedEndDate: string
    ownershipType: OwnershipType
    ownershipPercentage: string
    member: Member
    property: Property;
}

export enum OwnershipType {
    Primary = 0,
    CoOwner = 1
}


export type EditableProperty = {
    id: string
    address: string
    unit: any
    city: string
    state: string
    zipCode: string
    country: string
    lotNumber: string
    squareFeet: number
    bedrooms: number
    bathrooms: number
    isRented: boolean
    assignedParking: string
    isSameAddress: boolean
    mailAddress: string
    mailUnit: string
    mailCity: string
    mailState: string
    mailZipCode: string
    mailCountry: string
}



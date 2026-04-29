import { SelectOption } from "./select"

export type  Document= {
  id: string
  title: string
  description: string
  scope: DocumentScope
  category: DocumentCategory
  fileName: string
  mimeType: string
  fileSizeBytes: number
  isSuperseded: boolean
  supersededById?: string
  uploadedByDisplayName: string
  uploadedAt: string
  notes: string
  presignedUrl? : DocumentPresignedUrl
}

export type DocumentUploadRequest={  
  title: string
  description?: string
  scope: DocumentScope
  category: DocumentCategory
  propertyId?: string
  propertyOwnershipId?: string 
  notes?: string
  supersededDocumentId?: string
}


export type DocumentScope=    
    | 'Public' 
    | 'Community'         // All active members of this Client
    | 'PropertyHistory'   // Current Primary owner + board + managers
    | 'OwnerTenure'       // Only that specific tenure's Primary owner + managers
 

export const DOCUMENT_SCOPES: DocumentScope[] = [
  'Public',
  'Community',
  'PropertyHistory',
  'OwnerTenure'
] as const;

export const DOCUMENT_SCOPE_LABELS: Record<DocumentScope, string> = {
  Public: 'Public (Visible to everyone)',
  Community: 'Community (All active members)',
  PropertyHistory: 'Property History (Owner + Board + Managers)',
  OwnerTenure: 'Owner Tenure (Specific owner + Managers)'
};

 
export type DocumentCategory=
    // Community
    | 'HOAGuidelines'
    | 'MeetingMinutes'
    | 'Budget'
    | 'Newsletter'
    | 'Rules' 
    | 'MeetingNotice'
    | 'Other' 
    // Property
    | 'DeedOrOwnership'
    | 'ViolationNotice'
    | 'MaintenanceRecord'
    | 'InspectionReport'
    | 'ArchitecturalChangeRequest'
    | 'TenantDocument'
    | 'RentalAgreement'
    | 'Miscellaneous'


export type DocumentPresignedUrl={
url : string,
mode : string,
expiresInMinutes :15
}

export type DocumentMode =
  | 'preview'
  | 'downdload' 


  // --- option lists (define outside the class or in a constants file) ---
export const communityOptions: SelectOption[] = [
  { value: 'HOAGuidelines',  label: 'HOA Guidelines' },
  { value: 'MeetingMinutes', label: 'Meeting Minutes' },
  { value: 'Budget',         label: 'Budget' },
  { value: 'Newsletter',     label: 'Newsletter' },
  { value: 'Rules',          label: 'Rules' },
  { value: 'MeetingNotice',  label: 'Meeting Notice' },
  { value: 'Other',          label: 'Other' },
];
export const ownerTenureOptions: SelectOption[] = [
  { value: 'DeedOrOwnership', label: 'Deed or Ownership' },
  { value: 'Miscellaneous',   label: 'Miscellaneous' },
];
export const propertyHistoryOptions: SelectOption[] = [
  { value: 'DeedOrOwnership',           label: 'Deed or Ownership' },
  { value: 'ViolationNotice',           label: 'Violation Notice' },
  { value: 'MaintenanceRecord',         label: 'Maintenance Record' },
  { value: 'InspectionReport',          label: 'Inspection Report' },
  { value: 'ArchitecturalChangeRequest',label: 'Architectural Change Request' },
  { value: 'TenantDocument',            label: 'Tenant Document' },
  { value: 'RentalAgreement',           label: 'Rental Agreement' },
  { value: 'Miscellaneous',             label: 'Miscellaneous' },
];

           

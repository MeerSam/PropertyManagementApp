INSERT INTO "Documents" ("id", "ClientId", "Title", "Description",
    "Scope", "Category",
    "PropertyId", "PropertyOwnershipId",
    "StorageKey", "FileName", "MimeType", "FileSizeBytes",
    "IsActive", "IsSuperseded", "SupersededById",
    "UploadedByUserId", "UploadedAt", "Notes"
) VALUES 
( 
    'a1000000-0000-0000-0000-000000000001',
    'bpi-id',
    'HOA Community Guidelines 2024',
    'Official rules and regulations for all residents.',
    0, 1,          -- Scope=Community, Category=HOAGuidelines
    NULL, NULL,
    'clients/bpi-id/community/guidelines/a1000000-hoa-guidelines-2024.pdf',
    'HOA-Guidelines-2024.pdf', 'application/pdf', 524288,
    true, false, NULL,
    'anna-id', '2024-01-15 09:00:00', 'Approved by board Jan 2024'
);

UPDATE Documents
SET StorageKey = 'clients/bpi-id/community/guidelines/a1000000-hoa-guidelines-2024.pdf'
WHERE id = 'a1000000-0000-0000-0000-000000000001'
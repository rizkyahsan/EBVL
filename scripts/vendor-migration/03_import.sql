/* CONTROLLED IMPORT TEMPLATE. Never run against Local_EBVLDb before final gate approval. */
SET NOCOUNT ON;
SET XACT_ABORT ON;

THROW 51000, 'Import is intentionally blocked until backup, dry-run, storage, SME status, and final approval gates pass.', 1;

/* Approved implementation sequence after removing the guard in a reviewed copy:
   dbo.VendorTypes/dbo.ContactTypes/dbo.DocumentTemplates -> dbo.Vendors
   -> dbo.VendorContacts/dbo.VendorDocuments
   -> registrations -> accounts/users -> logical vendor children.
   Use INSERT ... SELECT with explicit keys and NOT EXISTS against Crosswalk.
   Conflicts go to Migration.Quarantine; no overwrite, deduplication, or MERGE. */

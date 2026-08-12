/* READ-ONLY. Run with least privilege. No secrets or PII are selected. */
SET NOCOUNT ON;

SELECT DB_NAME() AS CurrentDatabase,
       DATABASEPROPERTYEX(DB_NAME(), 'Collation') AS DatabaseCollation;

SELECT name, state_desc, recovery_model_desc
FROM sys.databases
WHERE name IN (DB_NAME(), N'Local_EBVLDb', N'DB_EBVL');

IF DB_ID(N'Local_EBVLDb') IS NOT NULL
BEGIN
    SELECT MigrationId, ProductVersion
    FROM [Local_EBVLDb].[EBVL].[__EFMigrationsHistory]
    ORDER BY MigrationId;

    SELECT s.name AS SchemaName, t.name AS TableName
    FROM [Local_EBVLDb].sys.tables t
    JOIN [Local_EBVLDb].sys.schemas s ON s.schema_id = t.schema_id
    WHERE t.name IN (N'Vendors', N'VendorTypes', N'VendorContacts', N'ContactTypes', N'VendorDocuments', N'DocumentTemplates', N'VendorUsers', N'Users', N'tempVendor', N'tempVendorDocument', N'VendorRegistrations', N'VendorAccounts')
      AND (s.name = N'dbo' AND t.name IN (N'Vendors', N'VendorTypes', N'VendorContacts', N'ContactTypes', N'VendorDocuments', N'DocumentTemplates')
           OR s.name = N'EBVL' AND t.name IN (N'VendorRegistrations', N'VendorRegistrationDocuments', N'VendorAccounts'));
END;

/* M004 collision preflight: any row returned by this query blocks execution. */
IF DB_ID(N'Local_EBVLDb') IS NOT NULL
SELECT n.TableName
FROM (VALUES (N'Vendors'), (N'VendorTypes'), (N'VendorContacts'), (N'VendorDocuments'), (N'ContactTypes'), (N'DocumentTemplates')) n(TableName)
WHERE OBJECT_ID(N'[EBVL].[' + n.TableName + N']', N'U') IS NOT NULL
  AND OBJECT_ID(N'[dbo].[' + n.TableName + N']', N'U') IS NOT NULL;

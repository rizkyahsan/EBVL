/* READ-ONLY validation. Run only after a disposable dry-run or approved import. */
SET NOCOUNT ON;

SELECT N'VendorRegistrations without canonical Vendor' AS CheckName, COUNT_BIG(*) AS IssueCount
FROM EBVL.VendorRegistrations WHERE VendorId IS NULL AND Status = 3;

SELECT N'Accounts without canonical Vendor' AS CheckName, COUNT_BIG(*) AS IssueCount
FROM EBVL.VendorAccounts WHERE VendorId IS NULL;

SELECT N'Accounts active without Active status' AS CheckName, COUNT_BIG(*) AS IssueCount
FROM EBVL.VendorAccounts WHERE IsActive = 1 AND (Status <> 1 OR IsDeleted = 1);

SELECT N'Null/truncation checks require source staging comparison' AS CheckName, CAST(0 AS bigint) AS IssueCount;

SELECT s.name AS SchemaName, t.name AS TableName, SUM(p.rows) AS ApproxRows
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0, 1)
WHERE (s.name = N'dbo' AND t.name IN (N'Vendors', N'VendorTypes', N'VendorContacts', N'ContactTypes', N'VendorDocuments', N'DocumentTemplates'))
   OR (s.name = N'EBVL' AND t.name IN (N'VendorRegistrations', N'VendorRegistrationDocuments', N'VendorAccounts'))
GROUP BY s.name, t.name
ORDER BY t.name;

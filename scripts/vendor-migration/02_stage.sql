/* Design-only staging contract. Execute only on a disposable dry-run database after approval. */
SET NOCOUNT ON;

IF SCHEMA_ID(N'Migration') IS NULL EXEC(N'CREATE SCHEMA Migration');

IF OBJECT_ID(N'Migration.RunLedger', N'U') IS NULL
CREATE TABLE Migration.RunLedger
(
    RunId uniqueidentifier NOT NULL PRIMARY KEY,
    SourceDatabase sysname NOT NULL,
    StartedAt datetimeoffset(7) NOT NULL,
    CompletedAt datetimeoffset(7) NULL,
    Mode nvarchar(20) NOT NULL,
    Status nvarchar(30) NOT NULL,
    Notes nvarchar(1000) NULL
);

IF OBJECT_ID(N'Migration.Crosswalk', N'U') IS NULL
CREATE TABLE Migration.Crosswalk
(
    RunId uniqueidentifier NOT NULL,
    SourceSystem nvarchar(100) NOT NULL,
    SourceTable sysname NOT NULL,
    SourceId nvarchar(100) NOT NULL,
    TargetTable sysname NOT NULL,
    TargetId uniqueidentifier NULL,
    MatchMethod nvarchar(50) NOT NULL,
    MatchConfidence nvarchar(20) NOT NULL,
    Decision nvarchar(20) NOT NULL,
    CONSTRAINT PK_MigrationCrosswalk PRIMARY KEY (SourceSystem, SourceTable, SourceId),
    CONSTRAINT FK_MigrationCrosswalk_Run FOREIGN KEY (RunId) REFERENCES Migration.RunLedger(RunId)
);

IF OBJECT_ID(N'Migration.Quarantine', N'U') IS NULL
CREATE TABLE Migration.Quarantine
(
    RunId uniqueidentifier NOT NULL,
    SourceTable sysname NOT NULL,
    SourceId nvarchar(100) NOT NULL,
    ReasonCode nvarchar(50) NOT NULL,
    Detail nvarchar(1000) NULL,
    CreatedAt datetimeoffset(7) NOT NULL,
    CONSTRAINT FK_MigrationQuarantine_Run FOREIGN KEY (RunId) REFERENCES Migration.RunLedger(RunId)
);

/* Target master objects are dbo.Vendors, dbo.VendorTypes, dbo.VendorContacts,
   dbo.VendorDocuments, dbo.ContactTypes, and dbo.DocumentTemplates.
   Workflow/account objects remain EBVL.*.
   Source rows must be copied into disposable staging tables by an approved tool.
   No MERGE and no source writes are permitted. */

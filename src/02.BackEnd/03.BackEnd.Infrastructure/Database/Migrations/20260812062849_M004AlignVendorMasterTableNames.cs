using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M004AlignVendorMasterTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET XACT_ABORT ON;

DECLARE @Collision nvarchar(2048) = N'';
IF OBJECT_ID(N'[EBVL].[Vendors]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[Vendors]', N'U') IS NOT NULL SET @Collision += N' Vendors';
IF OBJECT_ID(N'[EBVL].[VendorTypes]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[VendorTypes]', N'U') IS NOT NULL SET @Collision += N' VendorTypes';
IF OBJECT_ID(N'[EBVL].[VendorContacts]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[VendorContacts]', N'U') IS NOT NULL SET @Collision += N' VendorContacts';
IF OBJECT_ID(N'[EBVL].[VendorDocuments]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[VendorDocuments]', N'U') IS NOT NULL SET @Collision += N' VendorDocuments';
IF OBJECT_ID(N'[EBVL].[ContactTypes]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[ContactTypes]', N'U') IS NOT NULL SET @Collision += N' ContactTypes';
IF OBJECT_ID(N'[EBVL].[DocumentTemplates]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[DocumentTemplates]', N'U') IS NOT NULL SET @Collision += N' DocumentTemplates';
IF @Collision <> N''
BEGIN
    DECLARE @CollisionMessage nvarchar(2048) = N'M004 collision: both EBVL and dbo vendor master tables exist:' + @Collision + N'. Reconciliation is required; no table was transferred.';
    THROW 51004, @CollisionMessage, 1;
END;

IF OBJECT_ID(N'[EBVL].[Vendors]', N'U') IS NULL OR OBJECT_ID(N'[EBVL].[VendorTypes]', N'U') IS NULL
   OR OBJECT_ID(N'[EBVL].[VendorContacts]', N'U') IS NULL OR OBJECT_ID(N'[EBVL].[VendorDocuments]', N'U') IS NULL
   OR OBJECT_ID(N'[EBVL].[ContactTypes]', N'U') IS NULL OR OBJECT_ID(N'[EBVL].[DocumentTemplates]', N'U') IS NULL
    THROW 51004, N'M004 source state invalid: one or more EBVL vendor master tables are missing. No table was transferred.', 1;

ALTER SCHEMA [dbo] TRANSFER [EBVL].[VendorTypes];
ALTER SCHEMA [dbo] TRANSFER [EBVL].[Vendors];
ALTER SCHEMA [dbo] TRANSFER [EBVL].[VendorContacts];
ALTER SCHEMA [dbo] TRANSFER [EBVL].[VendorDocuments];
ALTER SCHEMA [dbo] TRANSFER [EBVL].[ContactTypes];
ALTER SCHEMA [dbo] TRANSFER [EBVL].[DocumentTemplates];
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET XACT_ABORT ON;

DECLARE @Collision nvarchar(2048) = N'';
IF OBJECT_ID(N'[dbo].[Vendors]', N'U') IS NOT NULL AND OBJECT_ID(N'[EBVL].[Vendors]', N'U') IS NOT NULL SET @Collision += N' Vendors';
IF OBJECT_ID(N'[dbo].[VendorTypes]', N'U') IS NOT NULL AND OBJECT_ID(N'[EBVL].[VendorTypes]', N'U') IS NOT NULL SET @Collision += N' VendorTypes';
IF OBJECT_ID(N'[dbo].[VendorContacts]', N'U') IS NOT NULL AND OBJECT_ID(N'[EBVL].[VendorContacts]', N'U') IS NOT NULL SET @Collision += N' VendorContacts';
IF OBJECT_ID(N'[dbo].[VendorDocuments]', N'U') IS NOT NULL AND OBJECT_ID(N'[EBVL].[VendorDocuments]', N'U') IS NOT NULL SET @Collision += N' VendorDocuments';
IF OBJECT_ID(N'[dbo].[ContactTypes]', N'U') IS NOT NULL AND OBJECT_ID(N'[EBVL].[ContactTypes]', N'U') IS NOT NULL SET @Collision += N' ContactTypes';
IF OBJECT_ID(N'[dbo].[DocumentTemplates]', N'U') IS NOT NULL AND OBJECT_ID(N'[EBVL].[DocumentTemplates]', N'U') IS NOT NULL SET @Collision += N' DocumentTemplates';
IF @Collision <> N''
BEGIN
    DECLARE @CollisionMessage nvarchar(2048) = N'M004 down collision: both dbo and EBVL vendor master tables exist:' + @Collision + N'. No table was transferred.';
    THROW 51004, @CollisionMessage, 1;
END;

IF OBJECT_ID(N'[dbo].[Vendors]', N'U') IS NULL OR OBJECT_ID(N'[dbo].[VendorTypes]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[VendorContacts]', N'U') IS NULL OR OBJECT_ID(N'[dbo].[VendorDocuments]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[ContactTypes]', N'U') IS NULL OR OBJECT_ID(N'[dbo].[DocumentTemplates]', N'U') IS NULL
    THROW 51004, N'M004 down state invalid: one or more dbo vendor master tables are missing. No table was transferred.', 1;

ALTER SCHEMA [EBVL] TRANSFER [dbo].[VendorTypes];
ALTER SCHEMA [EBVL] TRANSFER [dbo].[Vendors];
ALTER SCHEMA [EBVL] TRANSFER [dbo].[VendorContacts];
ALTER SCHEMA [EBVL] TRANSFER [dbo].[VendorDocuments];
ALTER SCHEMA [EBVL] TRANSFER [dbo].[ContactTypes];
ALTER SCHEMA [EBVL] TRANSFER [dbo].[DocumentTemplates];
");
        }
    }
}

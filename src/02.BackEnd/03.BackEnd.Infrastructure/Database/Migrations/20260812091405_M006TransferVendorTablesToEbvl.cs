using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M006TransferVendorTablesToEbvl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET XACT_ABORT ON;
DECLARE @Collision nvarchar(2048) = N'';
IF OBJECT_ID(N'[dbo].[VendorTypes]', N'U') IS NULL OR OBJECT_ID(N'[dbo].[Vendors]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[VendorContacts]', N'U') IS NULL OR OBJECT_ID(N'[dbo].[VendorDocuments]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[ContactTypes]', N'U') IS NULL OR OBJECT_ID(N'[dbo].[DocumentTemplates]', N'U') IS NULL
    THROW 51006, N'M006 source state invalid: one or more dbo vendor tables are missing.', 1;
IF OBJECT_ID(N'[EBVL].[VendorTypes]', N'U') IS NOT NULL SET @Collision += N' VendorTypes';
IF OBJECT_ID(N'[EBVL].[Vendors]', N'U') IS NOT NULL SET @Collision += N' Vendors';
IF OBJECT_ID(N'[EBVL].[VendorContacts]', N'U') IS NOT NULL SET @Collision += N' VendorContacts';
IF OBJECT_ID(N'[EBVL].[VendorDocuments]', N'U') IS NOT NULL SET @Collision += N' VendorDocuments';
IF OBJECT_ID(N'[EBVL].[ContactTypes]', N'U') IS NOT NULL SET @Collision += N' ContactTypes';
IF OBJECT_ID(N'[EBVL].[DocumentTemplates]', N'U') IS NOT NULL SET @Collision += N' DocumentTemplates';
IF @Collision <> N''
BEGIN
    DECLARE @CollisionMessage nvarchar(2048) = N'M006 collision: EBVL vendor tables already exist:' + @Collision;
    THROW 51006, @CollisionMessage, 1;
END;
ALTER SCHEMA [EBVL] TRANSFER [dbo].[VendorTypes];
ALTER SCHEMA [EBVL] TRANSFER [dbo].[Vendors];
ALTER SCHEMA [EBVL] TRANSFER [dbo].[VendorContacts];
ALTER SCHEMA [EBVL] TRANSFER [dbo].[VendorDocuments];
ALTER SCHEMA [EBVL] TRANSFER [dbo].[ContactTypes];
ALTER SCHEMA [EBVL] TRANSFER [dbo].[DocumentTemplates];
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET XACT_ABORT ON;
DECLARE @Collision nvarchar(2048) = N'';
IF OBJECT_ID(N'[EBVL].[VendorTypes]', N'U') IS NULL OR OBJECT_ID(N'[EBVL].[Vendors]', N'U') IS NULL
   OR OBJECT_ID(N'[EBVL].[VendorContacts]', N'U') IS NULL OR OBJECT_ID(N'[EBVL].[VendorDocuments]', N'U') IS NULL
   OR OBJECT_ID(N'[EBVL].[ContactTypes]', N'U') IS NULL OR OBJECT_ID(N'[EBVL].[DocumentTemplates]', N'U') IS NULL
    THROW 51006, N'M006 down state invalid: one or more EBVL vendor tables are missing.', 1;
IF OBJECT_ID(N'[dbo].[VendorTypes]', N'U') IS NOT NULL SET @Collision += N' VendorTypes';
IF OBJECT_ID(N'[dbo].[Vendors]', N'U') IS NOT NULL SET @Collision += N' Vendors';
IF OBJECT_ID(N'[dbo].[VendorContacts]', N'U') IS NOT NULL SET @Collision += N' VendorContacts';
IF OBJECT_ID(N'[dbo].[VendorDocuments]', N'U') IS NOT NULL SET @Collision += N' VendorDocuments';
IF OBJECT_ID(N'[dbo].[ContactTypes]', N'U') IS NOT NULL SET @Collision += N' ContactTypes';
IF OBJECT_ID(N'[dbo].[DocumentTemplates]', N'U') IS NOT NULL SET @Collision += N' DocumentTemplates';
IF @Collision <> N''
BEGIN
    DECLARE @CollisionMessage nvarchar(2048) = N'M006 down collision:' + @Collision;
    THROW 51006, @CollisionMessage, 1;
END;
ALTER SCHEMA [dbo] TRANSFER [EBVL].[VendorTypes];
ALTER SCHEMA [dbo] TRANSFER [EBVL].[Vendors];
ALTER SCHEMA [dbo] TRANSFER [EBVL].[VendorContacts];
ALTER SCHEMA [dbo] TRANSFER [EBVL].[VendorDocuments];
ALTER SCHEMA [dbo] TRANSFER [EBVL].[ContactTypes];
ALTER SCHEMA [dbo] TRANSFER [EBVL].[DocumentTemplates];
");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using EBVL.BackEnd.Domain.Entities;

namespace EBVL.BackEnd.Services.Database;

public partial interface IDatabaseService
{
    public DbSet<ApiCall> ApiCalls { get; }
    public DbSet<Audit> Audits { get; }
    public DbSet<Configuration> Configurations { get; }
    public DbSet<Country> Countries { get; }
    public DbSet<Document> Documents { get; }
    public DbSet<PublicHoliday> PublicHolidays { get; }
    public DbSet<User> Users { get; }
}

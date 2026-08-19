using EBVL.BackEnd.Infrastructure;
using EBVL.BackEnd.Infrastructure.AppConfigBackEnd;
using EBVL.BackEnd.Infrastructure.BackgroundJob;
using EBVL.BackEnd.Infrastructure.Database;
using EBVL.BackEnd.Infrastructure.HealthCheck;
using EBVL.BackEnd.Infrastructure.LocalIdentity;
using EBVL.BackEnd.Infrastructure.PathBase;
using EBVL.BackEnd.Infrastructure.Secret;
using EBVL.BackEnd.Logics;
using Microsoft.AspNetCore.HttpOverrides;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var appConfigBackEndOptions = builder.GetAppConfigBackEndOptions();
var secrets = await builder.GetSecretsAsync();
var servicesDatabaseKey = builder.Environment.IsDevelopment()
    ? SecretKeyFor.ConnectionStringsServicesDatabaseLocal
    : SecretKeyFor.ConnectionStringsServicesDatabase;
await ConfigureBackgroundJob.EnsureLocalBackgroundJobDatabaseAsync(secrets[servicesDatabaseKey]);
builder.AddInfrastructure(appConfigBackEndOptions, secrets);
builder.Services.AddLogics(builder.Configuration);

var app = builder.Build();
await app.InitializeLocalIdentityDatabase();
await app.InitializeDatabase(appConfigBackEndOptions.IsDataSeedingEnabled);
// Ensure container exists at startup
await app.Services.EnsureBlobContainerAsync();
app.UseExceptionHandler();
app.UseAndCheckPathBase(appConfigBackEndOptions.PathBase);

// ✅ New approach for http forwarded headers
var options = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
options.KnownProxies.Clear();
options.RequireHeaderSymmetry = false; // optional, helps with Azure
app.UseForwardedHeaders(options);

app.UseHttpsRedirection();
app.UseHsts();

app.UseHealthCheckService(appConfigBackEndOptions.PathBase);
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseBackgroundJobService(appConfigBackEndOptions.PathBase, secrets[SecretKeyFor.BackgroundJobDashboardUsername], secrets[SecretKeyFor.BackgroundJobDashboardKataKunci]);
app.RegisterRecurringJobs();
app.MapOpenApi();
app.MapScalarApiReference();
app.RegisterEndpoints(typeof(Program).Assembly);
await app.RunAsync();

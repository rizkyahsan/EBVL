using EBVL.FrontEnd.Infrastructure;
using EBVL.FrontEnd.Infrastructure.Authentication;
using EBVL.FrontEnd.Infrastructure.PathBase;
using EBVL.FrontEnd.Infrastructure.Secret;
using EBVL.FrontEnd.Logics;
using EBVL.FrontEnd.WebUi;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
var appConfigFrontEndOptions = builder.GetAppConfigFrontEndOptions();
var secrets = await builder.GetSecretsAsync();
builder.AddInfrastructure(appConfigFrontEndOptions, secrets);
builder.Services.AddLogics(builder.Configuration);
builder.AddWebUi();

//For Show error, Comment if not needed again
//builder.Services.AddServerSideBlazor()
//    .AddCircuitOptions(options =>
//    {
//        options.DetailedErrors = true;
//    });

var app = builder.Build();
app.UseAndCheckPathBase(appConfigFrontEndOptions.PathBase);

// Forwarded headers FIRST so scheme is correct
var options = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
options.KnownProxies.Clear();
options.RequireHeaderSymmetry = false; // useful for Azure load balancers
app.UseForwardedHeaders(options);

// Enforce HTTPS after scheme is fixed
app.UseHttpsRedirection();
app.UseHsts();

// Error handling
app.UseExceptionHandler($"/{MainRouteFor.Error}", createScopeForErrors: true);
app.UseStatusCodePagesWithReExecute("/ErrorWithCode/{0}");

// Security & auth
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Static + UI
app.MapStaticAssets();
app.MapBlazorHubWithPathBase(appConfigFrontEndOptions.PathBase);
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapAuthenticationEndpoints(appConfigFrontEndOptions.PathBase);

await app.RunAsync();

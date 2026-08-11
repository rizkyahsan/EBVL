using EBVL.FrontEnd.Infrastructure;
using EBVL.FrontEnd.Infrastructure.Authentication;
using EBVL.FrontEnd.Infrastructure.PathBase;
using EBVL.FrontEnd.Infrastructure.Secret;
using EBVL.FrontEnd.Logics;
using EBVL.FrontEnd.WebUi;

var builder = WebApplication.CreateBuilder(args);
var appConfigFrontEndOptions = builder.GetAppConfigFrontEndOptions();
var secrets = await builder.GetSecretsAsync();
builder.AddInfrastructure(appConfigFrontEndOptions, secrets);
builder.Services.AddLogics(builder.Configuration);
builder.AddWebUi();

var app = builder.Build();
app.UseAndCheckPathBase(appConfigFrontEndOptions.PathBase);
app.UseExceptionHandler($"/{MainRouteFor.Error}", createScopeForErrors: true);
app.UseHsts();
app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/ErrorWithCode/{0}");
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapBlazorHubWithPathBase(appConfigFrontEndOptions.PathBase);
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapAuthenticationEndpoints(appConfigFrontEndOptions.PathBase);
await app.RunAsync();

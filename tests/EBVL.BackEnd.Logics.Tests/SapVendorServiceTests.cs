using System.Net;
using System.Text;
using EBVL.BackEnd.Infrastructure.SapVendor;
using EBVL.BackEnd.Services.SapVendor;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace EBVL.BackEnd.Logics.Tests;

public sealed class SapVendorServiceTests
{
    [Fact]
    public async Task DisabledAdapterReturnsExplicitManualFallback()
    {
        var service = CreateService(new SapVendorOptions { Enabled = false });

        var result = await service.LookupAsync("10001", TestContext.Current.CancellationToken);

        Assert.Equal(SapVendorLookupStatus.Disabled, result.Status);
        Assert.Contains("manual", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AdapterReturnsCanonicalNumberFromSoapResponse()
    {
        const string Response = "<Envelope><Body><mt_GetEquiDet_Res><EQUIPMENT>000010001</EQUIPMENT></mt_GetEquiDet_Res></Body></Envelope>";
        var service = CreateService(new SapVendorOptions
        {
            Enabled = true,
            EndpointUrl = "https://sap.example.test/vendor",
            RequestNamespace = "urn:test",
            DefaultClient = "170"
        }, Response);

        var result = await service.LookupAsync("10001", TestContext.Current.CancellationToken);

        Assert.Equal(SapVendorLookupStatus.Found, result.Status);
        Assert.Equal("000010001", result.SapVendorNumber);
    }

    private static SapVendorService CreateService(SapVendorOptions options, string response = "")
    {
        var handler = new StubHttpMessageHandler(response);
        return new SapVendorService(new HttpClient(handler), Options.Create(options), NullLogger<SapVendorService>.Instance);
    }

    private sealed class StubHttpMessageHandler(string response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response, Encoding.UTF8, "text/xml")
            });
        }
    }
}

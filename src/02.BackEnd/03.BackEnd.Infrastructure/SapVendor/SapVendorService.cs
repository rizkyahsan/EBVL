using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using EBVL.BackEnd.Services.SapVendor;

namespace EBVL.BackEnd.Infrastructure.SapVendor;

public sealed class SapVendorService(HttpClient httpClient, IOptions<SapVendorOptions> options, ILogger<SapVendorService> logger) : ISapVendorService
{
    private readonly SapVendorOptions _options = options.Value;

    public async Task<SapVendorLookupResult> LookupAsync(string sapVendorNumber, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return new(SapVendorLookupStatus.Disabled, null, "SAP lookup is disabled; continue with manual entry.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.EndpointUrl);
        _ = request.Headers.TryAddWithoutValidation("SOAPAction", _options.SoapAction);
        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            var credential = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.Username}:{_options.Password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credential);
        }

        var requestNamespace = XNamespace.Get(_options.RequestNamespace);
        var soapNamespace = XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/");
        var envelope = new XDocument(
            new XElement(soapNamespace + "Envelope",
                new XElement(soapNamespace + "Body",
                    new XElement(requestNamespace + "mt_GetEquiDet_Req",
                        new XElement("client", _options.DefaultClient),
                        new XElement("equipment", sapVendorNumber.Trim())))));
        request.Content = new StringContent(envelope.ToString(SaveOptions.DisableFormatting), Encoding.UTF8, "text/xml");

        try
        {
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("SAP Vendor lookup failed with status {StatusCode}", (int)response.StatusCode);
                return new(SapVendorLookupStatus.Unavailable, null, "SAP lookup is temporarily unavailable; continue with manual entry.");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
            var number = document.Descendants()
                .FirstOrDefault(element => element.Name.LocalName.Equals("EQUIPMENT", StringComparison.OrdinalIgnoreCase))?
                .Value.Trim();

            return string.IsNullOrWhiteSpace(number)
                ? new(SapVendorLookupStatus.NotFound, null, "SAP vendor number was not found; continue with manual entry.")
                : new(SapVendorLookupStatus.Found, number);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("SAP Vendor lookup timed out");
            return new(SapVendorLookupStatus.Unavailable, null, "SAP lookup timed out; continue with manual entry.");
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(exception, "SAP Vendor lookup communication failure");
            return new(SapVendorLookupStatus.Unavailable, null, "SAP lookup is temporarily unavailable; continue with manual entry.");
        }
    }
}

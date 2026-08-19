using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using EBVL.FrontEnd.Infrastructure.Authentication.Statics;
using EBVL.FrontEnd.Services.BackEndApi;
using Pertamina.Common.Statics;
using Pertamina.Extensions.Identity;
using RestSharp;

namespace EBVL.FrontEnd.Infrastructure.BackEndApi;

public sealed class BackEndApiService(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor,
    NavigationManager navigationManager)
    : IBackEndApiService
{
    private readonly RestClient _restClient = new(httpClient);
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task SendRequestAsync(RestRequest restRequest, CancellationToken cancellationToken = default)
    {
        var isExpired = await GetExpirationInfoAsync();

        if (isExpired)
        {
            navigationManager.NavigateTo(new Uri(navigationManager.Uri).AbsolutePath, true);

            return;
        }

        await AddAuthorizationHeaderAsync(restRequest);

        var restResponse = await _restClient.ExecuteAsync(restRequest, cancellationToken);

        if (!restResponse.IsSuccessful)
        {
            HandleProblem(restResponse, restRequest);
        }
    }

    public async Task<T> SendRequestAsync<T>(RestRequest restRequest, CancellationToken cancellationToken = default)
    {
        var isExpired = await GetExpirationInfoAsync();

        if (isExpired)
        {
            navigationManager.NavigateTo(new Uri(navigationManager.Uri).AbsolutePath, true);

            throw new OperationCanceledException("Call to BackEnd API is canceled because the Access Token is expired.", cancellationToken);
        }

        await AddAuthorizationHeaderAsync(restRequest);

        var restResponse = await _restClient.ExecuteAsync<T>(restRequest, cancellationToken);

        if (!restResponse.IsSuccessful)
        {
            HandleProblem(restResponse, restRequest);
        }

        if (restResponse.Data is null)
        {
            throw ExceptionFor.JsonDeserializationFailed(restResponse.Content!, typeof(T));
        }

        return restResponse.Data;
    }

    private async Task<bool> GetExpirationInfoAsync()
    {
        var expiresAt = await _httpContext.GetTokenAsync(TokenNameFor.ExpiresAt);

        if (expiresAt is null)
        {
            return false;
        }

        if (!DateTimeOffset.TryParse(expiresAt, CultureInfo.InvariantCulture, out var expires))
        {
            throw new InvalidOperationException($"Failed to parse {expiresAt} into {nameof(DateTimeOffset)} instance.");
        }

        return DateTimeOffset.Now.LocalDateTime >= expires.LocalDateTime;
    }

    private async Task AddAuthorizationHeaderAsync(RestRequest restRequest)
    {
        var tokenType = await _httpContext.GetTokenAsync(TokenNameFor.TokenType);
        var accessToken = await _httpContext.GetTokenAsync(TokenNameFor.AccessToken);

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            _ = restRequest.AddHeader(KnownHeaders.Authorization, $"{tokenType} {accessToken}");
        }

        var positionId = _httpContext.User.GetPositionId();

        if (!string.IsNullOrWhiteSpace(positionId))
        {
            _ = restRequest.AddHeader(HttpHeaderNameFor.PositionId, positionId);
        }
    }

    private void HandleProblem(RestResponse restResponse, RestRequest restRequest)
    {
        if (string.IsNullOrWhiteSpace(restResponse.Content))
        {
            var uri = _restClient.BuildUri(restRequest);

            throw new ApiCallFailedException(uri.ToString(), (int)restResponse.StatusCode);
        }

        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(restResponse.Content, _jsonSerializerOptions)
                         ?? throw ExceptionFor.JsonDeserializationFailed(restResponse.Content, typeof(ProblemDetails));

        if (string.IsNullOrWhiteSpace(problemDetails.Detail))
        {
            problemDetails.Detail = "An error occurred while processing your request.";
        }

        var validationFailuresValue = problemDetails.Extensions
            .Where(x => x.Key == nameof(ModelValidationException.ValidationFailures))
            .Select(x => x.Value)
            .FirstOrDefault();

        if (validationFailuresValue is not null)
        {
            var validationFailures = JsonSerializer.Deserialize<IDictionary<string, string[]>>(validationFailuresValue.ToString()!, _jsonSerializerOptions);

            throw new ModelValidationException(validationFailures!);
        }

        var problemType = problemDetails.Type;

        if (string.IsNullOrWhiteSpace(problemType))
        {
            throw new InvalidOperationException(problemDetails.Detail);
        }

        var exceptionType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => x.FullName == problemType && typeof(Exception).IsAssignableFrom(x));

        if (exceptionType is null || !typeof(Exception).IsAssignableFrom(exceptionType))
        {
            throw new InvalidOperationException(problemDetails.Detail);
        }

        var exceptionObject = Activator.CreateInstance(exceptionType, problemDetails.Detail)
            ?? throw new InvalidOperationException(problemDetails.Detail);

        var exception = (Exception)exceptionObject;

        throw exception;
    }
}

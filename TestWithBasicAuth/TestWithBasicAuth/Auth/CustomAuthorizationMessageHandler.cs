using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Net;

namespace TestWithBasicAuth.Auth;

public class CustomAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly TokenService _tokenService;

    public CustomAuthorizationMessageHandler(ILocalStorageService localStorage, TokenService tokenService)
    {
        _localStorage = localStorage;
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Hämta access token från LocalStorage
        var accessToken = await _localStorage.GetItemAsync<string>("accessToken");

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Förnya access token
            Console.WriteLine("401 Unauthorized - Attempting token refresh...");

            var newAccessToken = await _tokenService.RefreshAccessTokenAsync();

            if (!string.IsNullOrWhiteSpace(newAccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }
}

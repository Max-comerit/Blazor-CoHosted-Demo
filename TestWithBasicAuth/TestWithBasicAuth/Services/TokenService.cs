using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using TestWithBasicAuth.Auth;

public class TokenService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public TokenService(
        HttpClient httpClient,
        ILocalStorageService localStorageService,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var loginDto = new LoginDto(username, password);
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7044/api/auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var tokenDto = await response.Content.ReadFromJsonAsync<TokenDto>();
                if (tokenDto != null)
                {
                    await _localStorageService.SetItemAsync("accessToken", tokenDto.AccessToken);
                    await _localStorageService.SetItemAsync("refreshToken", tokenDto.RefreshToken);

                    Console.WriteLine($"Access token saved: {tokenDto.AccessToken}");

                    if (_authenticationStateProvider is CustomAuthenticationStateProvider authProvider)
                    {
                        authProvider.NotifyUserAuthentication(tokenDto.AccessToken);
                    }

                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during login: {ex.Message}");
            return false;
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var token = await _localStorageService.GetItemAsync<string>("accessToken");
        Console.WriteLine($"Retrieved token: {token}");
        return token;
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        var token = await _localStorageService.GetItemAsync<string>("refreshToken");
        Console.WriteLine($"Retrieved refresh token: {token}");
        return token;
    }

    public async Task<(string AccessToken, string RefreshToken)> CheckTokensAsync()
    {
        var accessToken = await GetAccessTokenAsync() ?? "No access token saved.";
        var refreshToken = await GetRefreshTokenAsync() ?? "No refresh token saved.";

        Console.WriteLine($"AccessToken: {accessToken}");
        Console.WriteLine($"RefreshToken: {refreshToken}");

        return (accessToken, refreshToken);
    }

    public async Task LogoutAsync()
    {
        await _localStorageService.RemoveItemAsync("accessToken");
        await _localStorageService.RemoveItemAsync("refreshToken");
        Console.WriteLine("Tokens cleared.");

        if (_authenticationStateProvider is CustomAuthenticationStateProvider authProvider)
        {
            authProvider.NotifyUserLogout();
        }
    }

    public async Task<string?> RefreshAccessTokenAsync()
    {
        var accessToken = await _localStorageService.GetItemAsync<string>("accessToken");
        var refreshToken = await _localStorageService.GetItemAsync<string>("refreshToken");

        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var response = await _httpClient.PostAsJsonAsync("/api/token/refresh", new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });

        if (response.IsSuccessStatusCode)
        {
            var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>();

            // Uppdatera tokens i LocalStorage
            await _localStorageService.SetItemAsync("accessToken", tokens.AccessToken);
            await _localStorageService.SetItemAsync("refreshToken", tokens.RefreshToken);

            return tokens.AccessToken;
        }

        return null;
    }
    private class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public async Task<double?> GetAccessTokenRemainingSecondsAsync()
    {
        var token = await _localStorageService.GetItemAsync<string>("accessToken");
        return GetRemainingSeconds(token);
    }
    public static double? GetRemainingSeconds(string tokenString)
    {
        if (string.IsNullOrWhiteSpace(tokenString))
            return null;

        var jwtHandler = new JwtSecurityTokenHandler();

        if (!jwtHandler.CanReadToken(tokenString))
            return null;

        var jwtToken = jwtHandler.ReadJwtToken(tokenString);
        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

        if (expClaim == null)
            return null;

        if (long.TryParse(expClaim, out long expSeconds))
        {
            var expDateUtc = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
            var nowUtc = DateTime.UtcNow;

            // Om token har gått ut returnera 0, annars antal sekunder kvar
            var remaining = expDateUtc - nowUtc;
            return remaining.TotalSeconds > 0 ? remaining.TotalSeconds : 0;
        }

        return null;
    }

    private record LoginDto(string UserName, string Password);
    private record TokenDto(string AccessToken, string RefreshToken);
}

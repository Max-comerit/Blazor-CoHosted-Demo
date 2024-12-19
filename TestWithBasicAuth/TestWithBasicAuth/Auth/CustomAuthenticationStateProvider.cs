namespace TestWithBasicAuth.Auth;

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

//Extract token access to own service example TokenService (get, save, remove)
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsStringAsync("accessToken");
        Console.WriteLine($"Retrieved Token: {token}");

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(_anonymous);
        }

        try
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing token: {ex.Message}");
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task NotifyUserLogout()
    {
        try
        {
            await Task.WhenAll(
                    _localStorage.RemoveItemAsync("accessToken").AsTask(),
                    _localStorage.RemoveItemAsync("refreshToken").AsTask());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing items from local storage: {ex.Message}");
            throw;
        }
           
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt))
        {
            throw new ArgumentException("JWT is null or empty.");
        }

        if (jwt.Split('.').Length != 3)
        {
            throw new ArgumentException("JWT is not in the correct format.");
        }

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}


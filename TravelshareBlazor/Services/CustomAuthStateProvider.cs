using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigation;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public CustomAuthStateProvider(ILocalStorageService localStorage, NavigationManager navigation)
    {
        _localStorage = localStorage;
        _navigation = navigation;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_currentUser.Identity is { IsAuthenticated: true })
        {
            return new AuthenticationState(_currentUser);
        }

        var token = await SafeGetTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            _currentUser = new ClaimsPrincipal(identity);
        }
        else
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        return new AuthenticationState(_currentUser);
    }

    public void NotifyUserAuthentication(string token)
{
    var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
    var user = new ClaimsPrincipal(identity);
    
    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
}

public void NotifyUserLogout()
{
    var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());

    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
}

    private IEnumerable<Claim> ParseClaimsFromJwt(string token)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwt.Claims;
    }

    private async Task<string?> SafeGetTokenAsync()
    {
        try
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }
        catch
        {
            return null;
        }
    }
}

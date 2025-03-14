using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TravelshareBlazor.Services;
using Microsoft.JSInterop;
using System.Threading.Tasks;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IJSRuntime _jsRuntime; 

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
        _jsRuntime = jsRuntime; 
    }

    public async Task<bool> Login(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });

        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        await _localStorage.SetItemAsync("authToken", result.Token);

        ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);

        await AttachToken();

        return true;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

     public async Task<bool> IsLoggedIn()
{
    if (_jsRuntime is null)
    {
        return false; 
    }

    var token = await _localStorage.GetItemAsync<string>("authToken");
    if (!string.IsNullOrEmpty(token))
    {
        await AttachToken();
        return true;
    }
    return false;
}



    private async Task AttachToken()
{
    var token = await _localStorage.GetItemAsync<string>("authToken");
    if (!string.IsNullOrEmpty(token))
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}

public async Task<string> GetAuthToken()
{
    return await _localStorage.GetItemAsync<string>("authToken");
}

}

public class AuthResponse
{
    public string Token { get; set; }
}

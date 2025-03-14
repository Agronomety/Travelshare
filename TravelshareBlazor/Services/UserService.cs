using System.Net.Http.Json;
using TravelshareBlazor.Models;

namespace TravelshareBlazor.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/users", user);
            return response.IsSuccessStatusCode;
        }
    }
}

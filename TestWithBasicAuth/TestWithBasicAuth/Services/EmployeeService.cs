using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using TestWithBasicAuth.Models;

namespace TestWithBasicAuth.Services
{
    public class EmployeeService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorageService;

        public EmployeeService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _localStorageService = localStorageService;
        }

        public async Task<List<EmployeeDto>?> GetEmployeesAsync(int companyId)
        {
            var client = _httpClientFactory.CreateClient("SecureApi");

            // Hämta token från LocalStorage
            var token = await _localStorageService.GetItemAsync<string>("accessToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("Token is missing.");
                return null;
            }

            // Lägg till token i Authorization-headern
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"api/companies/{companyId}/employees");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Content: {content}"); // Logga svaret

                    return await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
                }
                else
                {
                    Console.WriteLine($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled Exception in GetEmployeesAsync: {ex.Message}");
            }

            return null;
        }
    }
}

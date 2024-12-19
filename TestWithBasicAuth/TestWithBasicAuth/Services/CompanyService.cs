using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using TestWithBasicAuth.Models;

namespace TestWithBasicAuth.Services
{
    public class CompanyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorageService;

        public CompanyService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _localStorageService = localStorageService;
        }

        public async Task<List<CompanyDto>?> GetCompaniesAsync()
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

            var response = await client.GetAsync("api/Companies?IncludeEmployees=true");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<List<CompanyDto>>();
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            return null;
        }

        public async Task<CompanyDto?> GetCompanyAsync()
        {
            try
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

                var response = await client.GetAsync("https://localhost:7044/api/companies/1");
                var statusCode = response.StatusCode.ToString();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content.ReadFromJsonAsync<CompanyDto>();
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine($"API Error: {statusCode} - {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled Exception: {ex.Message}");
                return null;
            }
        }
    }
}

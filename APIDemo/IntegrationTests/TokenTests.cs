using Companies.API;
using Companies.Infrastructure.Data;
using Companies.Shared.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegrationTests;
public class TokenTests  : IClassFixture<WebApplicationFactory<Program>>
{
    private HttpClient httpClient;
    private JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    //  private IServiceManager serviceManager;
    private CompaniesContext db;

    public TokenTests(WebApplicationFactory<Program> applicationFactory)
    {
        applicationFactory.ClientOptions.BaseAddress = new Uri("https://localhost:7044/api/");
        httpClient = applicationFactory.CreateClient();

        var scope = applicationFactory.Services.CreateScope();
        db = scope.ServiceProvider.GetRequiredService<CompaniesContext>();
    }

    [Fact]
    public async Task GetTokenAndTryToGetResponse()
    {

       // var exspectedCount = db.Companies.Count(); //(await serviceManager.CompanyService.GetCompaniesAsync(false, false)).Count();
        var res = await httpClient.PostAsJsonAsync("auth/login", new UserForAuthDto(UserName: "Kalle",  PassWord: "ABC123"));

        res.EnsureSuccessStatusCode();
        var tokenString = await res.Content.ReadAsStringAsync();
        var tokenDto = JsonSerializer.Deserialize<TokenDto>(tokenString, options);

        var request = new HttpRequestMessage(HttpMethod.Get, "companies");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto?.AccessToken);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var dtos = JsonSerializer.Deserialize<IEnumerable<CompanyDto>>(content, options);

        Assert.IsAssignableFrom<IEnumerable<CompanyDto>>(dtos);
        Assert.Equal(2, dtos?.Count());


    }

}

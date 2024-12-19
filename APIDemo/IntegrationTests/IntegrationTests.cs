using Companies.API;
using Companies.Infrastructure.Data;
using Companies.Shared.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace IntegrationTests;

public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private HttpClient httpClient;
    private CompaniesContext context;
    private JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public IntegrationTests(CustomWebApplicationFactory applicationFactory)
    {
        applicationFactory.ClientOptions.BaseAddress = new Uri("https://localhost:7044/api/");

        httpClient = applicationFactory.CreateClient();
        context = applicationFactory.Context;
    }

    [Fact]
    public async void IntegrationTest()
    {
        var response = await httpClient.GetAsync("demo");

        var result = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("Hello from controller", result);
        Assert.Equal("text/plain", response.Content.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task Index_ShouldRetrurnExpectedMediType()
    {
        var response = await httpClient.GetAsync("demo/dto");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        var dto = JsonSerializer.Deserialize<CompanyDto>(result, options);

        Assert.Equal("Working", dto?.Name);
        Assert.Equal("application/json", response.Content.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task Index_ShouldRetrurnExpectedMediType2()
    {
        var dto = await httpClient.GetFromJsonAsync<CompanyDto>("demo/dto");
        Assert.Equal("Working", dto?.Name);
    }

    //No auth in this see TokenTests
    //[Fact]
    //public async Task Index_ShouldRetrurnExpectedMediType3()
    //{
    //    var expectedCount = context.Companies.Count();
    //    //var dtos = await httpClient.GetFromJsonAsync<IEnumerable<CompanyDto>>("demo/getall", options);
    //    var request = new HttpRequestMessage(HttpMethod.Get, "companies");
    //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    //    var response = await httpClient.SendAsync(request);
    //    response.EnsureSuccessStatusCode();

    //    var content = await response.Content.ReadAsStringAsync();
    //    var dtos = JsonSerializer.Deserialize<IEnumerable<CompanyDto>>(content, options);

    //    Assert.NotNull(dtos);
    //    Assert.Equal(2, dtos?.Count());
    }


}
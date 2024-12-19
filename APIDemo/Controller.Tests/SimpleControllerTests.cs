using Companies.API.DTOs;
using Companies.Presemtation.Controllers;
using Companies.Shared.DTOs;
using Controller.Tests.Extensions;
using Controller.Tests.TestFixtures;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Controller.Tests;

[Collection("DatabaseCollection")]
public class SimpleControllerTests //: IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public SimpleControllerTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    //Todo: Fix!

    [Fact]
    public async void GetCompany_Should_Return400()
    {
        //Same problem here sideeffects from previous tests
        var sut = fixture.Sut;
        sut.SetUserIsAuth(false);

        var res = await sut.GetCompany();
        var resultType = res.Result as BadRequestObjectResult;

        Assert.IsType<BadRequestObjectResult>(resultType);
        Assert.Equal(StatusCodes.Status400BadRequest, resultType.StatusCode);

    }

    [Fact]
    public async Task GetCompany_IfNotAuth_ShouldReturd400BadRequest()
    {
        var httpContext = Mock.Of<HttpContext>(x => x.User.Identity.IsAuthenticated == false);

        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext,
        };

        var sut = fixture.Sut;
        sut.ControllerContext = controllerContext;

        var res = await sut.GetCompany();
        var resultType = res.Result as BadRequestObjectResult;

        Assert.IsType<BadRequestObjectResult>(resultType);
        Assert.Equal("is not auth", resultType.Value);
    }

    [Fact]
    public async Task GetCompany_IfNotAuth_ShouldReturn400BadRequest2()
    {
        var sut = fixture.Sut;
        sut.SetUserIsAuth(false);


        var result = await sut.GetCompany();
        var resultType = result.Result as BadRequestObjectResult;

        Assert.IsType<BadRequestObjectResult>(resultType);
        Assert.Equal(StatusCodes.Status400BadRequest, resultType.StatusCode);


    }

    [Fact]
    public async Task GetCompany_Auth_ShouldReturn200()
    {
        var sut = fixture.Sut;
        sut.SetUserIsAuth(true);

        var result = await sut.GetCompany();
        var resultType = result.Result as OkObjectResult;

        Assert.IsType<OkObjectResult>(resultType);
    }

    [Fact]
    public async Task GetCompany_ShouldReturnExpectedCount()
    {
        var sut = fixture.Sut;
        var expectedCount = fixture.Context.Companies.Count();

        var result = await sut.GetCompany2();

        var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsType<List<CompanyDto>>(okObjectResult.Value);

        Assert.Equal(expectedCount, items.Count);

    }

}

//public interface IUserService
//{
//    Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);
//    Task<bool> IsInRoleAsync(ApplicationUser user, string role);
//}

//public class UserService : IUserService
//{
//    private readonly UserManager<ApplicationUser> _userManager;

//    public UserService(UserManager<ApplicationUser> userManager)
//    {
//        _userManager = userManager;
//    }

//    public async Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal)
//    {
//        return await _userManager.GetUserAsync(principal);
//    }

//    public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
//    {
//        return await _userManager.IsInRoleAsync(user, role);
//    }
//}
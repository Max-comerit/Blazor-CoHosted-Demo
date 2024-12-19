using AutoMapper;
using Companies.Infrastructure.Data;
using Companies.Presemtation.ControllersForTestDemo;
using Companies.Shared.DTOs;
using Controller.Tests.Extensions;
using Controller.Tests.TestFixtures;
using Domain.Contracts;
using Domain.Models.Entities;
using Domain.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Tests;
public class RepoControollerTest : IClassFixture<RepoControllerFixture>
{
   
    private const string userName = "Kalle";
    private readonly RepoControllerFixture fixture;

    public RepoControollerTest(RepoControllerFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task GetEmployees_ShouldReturnAllEmplyees()
    {
        var users = fixture.GetUsers();
        var dtos = fixture.Mapper.Map<IEnumerable<EmployeeDto>>(users);
        ApiBaseResponse baseResponse = new ApiOkResponse<IEnumerable<EmployeeDto>>(dtos);

        fixture.ServiceManagerMock.Setup(x => x.EmployeeService.GetEmployeesAsync(1)).ReturnsAsync(baseResponse);
        fixture.UserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { UserName = userName });

       //U sut.SetUserIsAuth(true);
        var result = await fixture.Sut.GetEmployees(1);
        //var resultType = result.Result as OkObjectResult;

        //Assert
        var okObjectResult =  Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsType<List<EmployeeDto>>(okObjectResult.Value);
      
        Assert.Equal(items.Count, users.Count);

    }


    
    [Fact]  
    public async Task GetEmployees_ShouldThrowExceptionIfUserNotFound()
    {
        //State from previous SetUp method. Methods dont run in parallel but order is not guaranted
        fixture.UserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser)null!);
        await Assert.ThrowsAsync<ArgumentNullException>(() => fixture.Sut.GetEmployees(1));
    }
}

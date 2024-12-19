using Companies.API.DTOs;
using Companies.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Companies.Presemtation.ControllersForTestDemo;
[ApiController]
[Route("api/demo")]
public class IntegrationController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public IntegrationController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    [HttpGet]
    public ActionResult Get()
    {
        return Ok("Hello from controller");
    }

    [HttpGet("dto")]
    public ActionResult Get2()
    {
        var dto = new CompanyDto { Name = "Working" };
        return Ok(dto);
    }

    [HttpGet("getall")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> Get3([FromQuery] CompanyRequestParams requestParams)
    {
        var result = await serviceManager.CompanyService.GetCompaniesAsync(requestParams);
        var dtos = result.companyDtos;
        return Ok(dtos.ToList());
    }
}

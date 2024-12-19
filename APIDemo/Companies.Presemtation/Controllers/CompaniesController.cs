using Companies.API.DTOs;
using Companies.Shared.Request;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Text.Json;

namespace Companies.Presemtation.Controllers;

[Route("api/Companies")]
[ApiController]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly IServiceManager serviceManager;
    private readonly UserManager<ApplicationUser> userManager;

    public CompaniesController(IServiceManager serviceManager, UserManager<ApplicationUser> userManager)
    {
        this.serviceManager = serviceManager;
        this.userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompany([FromQuery]CompanyRequestParams requestParams)
    {
        
        var pagedReault = await serviceManager.CompanyService.GetCompaniesAsync(requestParams);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedReault.metaData));

        return Ok(pagedReault.companyDtos);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CompanyDto>> GetCompany(int id) =>
            Ok(await serviceManager.CompanyService.GetCompanyAsync(id));


    //// PUT: api/Companies/5
    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPut("{id}")]
    //public async Task<IActionResult> PutCompany(int id, CompanyUpdateDto dto)
    //{
    //    if (id != dto.Id) return BadRequest();

    //    var existingCompany = await uow.CompanyRepository.GetCompanyAsync(id, trackChanges: true);
    //    if(existingCompany == null) return NotFound();

    //    _mapper.Map(dto, existingCompany);
    //    await uow.CompleteASync();

    //   // return NoContent();
    //   return Ok(_mapper.Map<CompanyDto>(existingCompany));
    //}

    //// POST: api/Companies
    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPost]
    //public async Task<IActionResult> PostCompany(CompanyCreateDto dto)
    //{

    //    var company = _mapper.Map<Company>(dto);
    //    uow.CompanyRepository.Create(company);
    //    await uow.CompleteASync();

    //    var createdCompany = _mapper.Map<CompanyDto>(company);

    //    return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.Id }, createdCompany);
    //}

    ////// DELETE: api/Companies/5
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteCompany(int id)
    //{
    //    var company = await uow.CompanyRepository.GetCompanyAsync(id);
    //    if (company == null) return NotFound();

    //    uow.CompanyRepository.Delete(company);
    //    await uow.CompleteASync();

    //    return NoContent();
    //}

}

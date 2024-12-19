using AutoMapper;
using Companies.API.DTOs;
using Companies.Infrastructure.Data;
using Companies.Shared.Request;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System.Text.Json;

namespace Companies.Presemtation.Controllers;

[Route("api/simple")]
[ApiController]
[Authorize]
public class SimpleController : ControllerBase
{
    private readonly CompaniesContext db;
    private readonly IMapper mapper;

    public SimpleController(CompaniesContext db, IMapper mapper)
    {
        this.db = db;
        this.mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompany()
    { 
        
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            return Ok("is auth");
        }
        else
        {
            return BadRequest("is not auth");
        }
    }

    [HttpGet("uniqueroute")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompany2()
    {
        var companies = await db.Companies.ToListAsync();
        var compDtos = mapper.Map<IEnumerable<CompanyDto>>(companies);
        return Ok(compDtos);
    }

}
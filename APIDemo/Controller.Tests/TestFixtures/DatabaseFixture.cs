using AutoMapper;
using Companies.Infrastructure.Data;
using Companies.Presemtation.Controllers;
using Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Tests.TestFixtures;
public class DatabaseFixture : IDisposable
{
    public CompaniesContext Context { get; }
    public SimpleController Sut { get; }

    public DatabaseFixture()
    {
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }));

        //var configuration = new ConfigurationBuilder()
        //       .SetBasePath(Directory.GetCurrentDirectory())
        //       .AddJsonFile("appsettings.json")
        //       .Build();

        var options = new DbContextOptionsBuilder<CompaniesContext>().UseInMemoryDatabase("TestDB").Options;

       Context  = new CompaniesContext(options);

        Sut = new SimpleController(Context, mapper);

     //   Context.Database.Migrate();
        SeedData();

        Context.SaveChanges();

    }

    private void SeedData()
    {
        Context.Companies.AddRange([
                                   new Company()
                                            {
                                              Name = "TestCompanyName",
                                              Address = "TestAdress",
                                              Country = "TestCountry",
                                              Employees =
                                                  [
                                                    new ApplicationUser
                                                    {
                                                        UserName = "TestUserName",
                                                        Email = "test@test.com",
                                                        Age = 50,
                                                        Name = "TestName",
                                                        Position = "TestPosition"
                                                    }
                                                  ]
                                            }
                                   ]);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}

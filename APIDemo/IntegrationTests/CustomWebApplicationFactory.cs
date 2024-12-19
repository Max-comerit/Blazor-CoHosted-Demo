using Companies.API;
using Companies.Infrastructure.Data;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public CompaniesContext Context { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {

            var dbContextOptions = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CompaniesContext>));

            if (dbContextOptions != null)
                services.Remove(dbContextOptions);

            services.AddDbContext<CompaniesContext>(options =>
            {
                options.UseInMemoryDatabase("T2");
            });

            var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CompaniesContext>();

            //Förenkling
            context.Companies.AddRange(
                               [
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

            context.SaveChanges();
            Context = context;

        });
    }

    public override ValueTask DisposeAsync()
    {
      //  Context.Database.EnsureDeleted();
        Context.Dispose();
        return base.DisposeAsync();
    }
}

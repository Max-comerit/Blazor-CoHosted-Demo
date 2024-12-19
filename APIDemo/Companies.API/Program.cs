using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Companies.API.Extensions;
using Companies.Infrastructure.Data;
using Companies.Infrastructure.Repositories;
using Domain.Contracts;
using Services.Contracts;
using Companies.Services;
using Companies.Presemtation;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace Companies.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<CompaniesContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CompaniesContext") ?? throw new InvalidOperationException("Connection string 'CompaniesContext' not found.")));

            builder.Services.AddControllers(configure =>
            {
                     configure.ReturnHttpNotAcceptable = true;

                //Global Filter
                //var policy = new AuthorizationPolicyBuilder()
                //                    .RequireAuthenticatedUser()
                //                    .RequireRole("Employee")
                //                    .Build();

                //configure.Filters.Add(new AuthorizeFilter(policy));


            })
                            // .AddXmlDataContractSerializerFormatters()
                            .AddNewtonsoftJson()
                            .AddApplicationPart(typeof(AssemblyReference).Assembly);

            builder.Services.ConfigureOpenApi();

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
           
            builder.Services.ConfigureServiceLayerServices();
            builder.Services.ConfigureRepositories();

            builder.Services.ConfigureJwt(builder.Configuration);

           


            builder.Services.AddIdentityCore<ApplicationUser>(opt =>
                {
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequiredLength = 3;
                    opt.User.RequireUniqueEmail = true;
                })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<CompaniesContext>()
                    .AddDefaultTokenProviders();


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                   policy.RequireRole("Admin")
                         .RequireClaim(ClaimTypes.NameIdentifier)
                         .RequireClaim(ClaimTypes.Role));

                options.AddPolicy("EmployeePolicy", policy =>
                    policy.RequireRole("Employee"));

            });




            builder.Services.ConfigureCors();
            builder.Services.AddControllersWithViews(); //Tillagd
            builder.Services.AddRazorPages(); //tillagd


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.ConfigureExceptionHandler();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                await app.SeedDataAsync();
                app.UseWebAssemblyDebugging(); //Tillagd

            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles(); //Tillagd
            app.UseStaticFiles(); //Tillagd


            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.MapFallbackToFile("index.html"); //tillagd

            app.Run();
        }
    }
}

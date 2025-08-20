using Library.Core;
using Library.Core.Entities.Identity;
using Library.Core.ServiceContract;
using Library.Repository;
using Library.Repository.Data;
using Library.Repository.Identity;
using Library.Service;
using LibraTrack.API.Extensions;
using LibraTrack.API.Helpers;
using LibraTrack.API.MiddleWares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using System.Threading.Tasks;
namespace LibraTrack.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configure Service
            builder.Services.AddControllers().
                AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddDbContext<LibraryContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<AppIdentityDbContext>(options=>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

            builder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            }
            );

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("account-v1", new OpenApiInfo { Title = "Account API", Version = "v1" });
                c.SwaggerDoc("admin-v1", new OpenApiInfo { Title = "Admin APIs", Version = "v1" });
                c.SwaggerDoc("public-v1", new OpenApiInfo { Title = "Public APIs", Version = "v1" });
                c.SwaggerDoc("shared-v1", new OpenApiInfo { Title = "Shared APIs", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Put Bearer Token Here"
                });

                c.OperationFilter<AuthorizeOperationFilter>();

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var group = apiDesc.GroupName;
                    return string.Equals(group, docName, StringComparison.OrdinalIgnoreCase);
                });
            });
            builder.Services.AddScoped<IResponseCashService, ResponseCashService>();


            builder.Services.AddApplicationServices();
            builder.Services.AddIdentityServices(builder.Configuration);

            #endregion

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<LibraryContext>();
            var _identityDbContext = services.GetRequiredService<AppIdentityDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                await _dbContext.Database.MigrateAsync();
                await _identityDbContext.Database.MigrateAsync();
                
                await AddRolesSeed.SeedRolesAsync(roleManager);
                await LibraryContextSeed.SeedAsync(_dbContext);
            }
            catch(Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
            // Configure the HTTP request pipeline.
            #region Configure Kestrel MiddleWares
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/account-v1/swagger.json", "Account API v1");
                    c.SwaggerEndpoint("/swagger/admin-v1/swagger.json", "Admin APIs v1");
                    c.SwaggerEndpoint("/swagger/public-v1/swagger.json", "Public APIs v1");
                    c.SwaggerEndpoint("/swagger/shared-v1/swagger.json", "Shared APIs v1");
                });
            }
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run(); 
            #endregion
        }
    }
}

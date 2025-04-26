using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sanayii.APIs.Extensions;
using Sanayii.APIs.Services;
using Sanayii.Core.Entities;
using Sanayii.Services;
using Sanayii.Repository.Data;
using System.Threading.Tasks;
using Sanayii.Core.Interfaces;
using Sanayii.Service.Chat;
using Sanayii.Core.DTOs.ChatDTOs;
using System.Text.Json.Nodes;
namespace Sanayii
{
    public class Program
    {
        // Entry Point
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region Configure Services
            // Add services to the container.
            builder.Services.AddControllers(); // Apply API Configurations
            //builder.Services.AddRazorPages(); // Add if using Razor Pages

            //logging
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);

            // Configure Swagger
            builder.Services.AddSwaggerServices();

            // Database Connection
            builder.Services.AddDbContext<SanayiiContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity Configuration
            builder.Services.AddIdentityServices(builder.Configuration);


            // Add Application Services
            builder.Services.AddApplicationServices();

            //Registeration for Api chat 
            
            // ???? ??????:
             //builder.Services.AddHttpClient<IChatService, ChatService>();
            // Add to your services configuration
            builder.Services.AddHttpClient<IChatService, ChatService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                return new ChatService(
                    provider.GetRequiredService<HttpClient>(),
                    config,
                    provider.GetRequiredService<ILogger<ChatService>>());
            });

            // CORS Configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecific",
                    policy =>
                    {
                        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    });
            });


            #endregion


            var app = builder.Build();


            #region Apply Migrations
            //using var scope = app.Services.CreateScope();

            //var services = scope.ServiceProvider;

            //var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            //try
            //{
            //    var dbContext = services.GetRequiredService<SanayiiContext>(); // Ask Explicitly
            //    await dbContext.Database.MigrateAsync(); // Apply Migrations [Update Database]

            //}
            //catch (Exception ex)
            //{
            //    var logger = loggerFactory.CreateLogger<Program>();
            //    logger.LogError(ex, ex.Message);
            //}
            #endregion



            

            
            #region Configure HTTP Requests Pipeline
            // Configure Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddlewares();
               
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.UseStaticFiles();

            app.UseCors("AllowSpecific");

            app.MapControllers();
            //app.MapRazorPages(); //  Required for Razor Pages

            #endregion


            app.Run();
        }
    }
}

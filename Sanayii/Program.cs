using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sanayii.Core.Entities;
using Sanayii.Core.Entities;
using Sanayii.Services;
using Snai3y.Repository.Data;
using System.Text;

namespace Sanayii
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddRazorPages(); //  Add if using Razor Pages

            // Configure Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database Connection
            builder.Services.AddDbContext<SanayiiContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<SignInManager<AppUser>>();
            builder.Services.AddTransient<EmailSenderService>();
            builder.Services.AddTransient<SMSSender>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            // Identity Configuration
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<SanayiiContext>()
            .AddDefaultTokenProviders();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(30);
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
       .AddJwtBearer(options =>
       {
           options.SaveToken = true;
           options.RequireHttpsMetadata = true; // Ensure HTTPS for security
           options.TokenValidationParameters = new TokenValidationParameters()
           {
               ValidateIssuer = true,
               ValidIssuer = "http://localhost:5127/",
               ValidateAudience = true, // Better security
               ValidAudience = "http://localhost:5127/", // Define a valid audience
               ValidateLifetime = true, // Ensure token expiration is checked
               ValidateIssuerSigningKey = true, // Validate secret key
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("L6scvGt8D3yU5vAqZt9PfMxW2jNkRgT7!@#$%")) // Use environment config
           };
       })
       .AddFacebook(options =>
       {
           options.AppId = "1786811492250782"; // Replace with your Facebook App ID
           options.AppSecret = "b31e78cb811d0ca8f9252697c512ce27";
           options.CallbackPath = "/signin-facebook";
       });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyAllowSpecificOrigins", policy =>
                {
                    policy.AllowAnyOrigin()  // ← السماح بأي مصدر (مؤقتًا للاختبار)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            var app = builder.Build();
            app.UseCors("MyAllowSpecificOrigins");


            // Configure Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = string.Empty; //  Open Swagger as default
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapRazorPages(); //  Required for Razor Pages

            app.Run();
        }
    }
}

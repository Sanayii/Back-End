using Microsoft.AspNetCore.Identity;
using Sanayii.APIs.Services;
using Sanayii.Core;
using Sanayii.Core.Repositories;
using Sanayii.Repository;
using Sanayii.Service.Mappers;
using Sanayii.Services;

namespace Sanayii.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            // SignIn Manager
            services.AddScoped<SignInManager<AppUser>>();

            // Email Sender Service
            services.AddTransient<EmailSenderService>();

            // SMS Sender Service
            services.AddTransient<SMSSenderService>();

            // Apply AutoMapper
            services.AddAutoMapper(typeof(CustomerMapper));

            // Apply Generic Repository Dependency Injection
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Apply Unit of Work Dependency Injection
            services.AddScoped<IUnitOfWork, UnitOfWork>();



            return services;
        }
    }
}

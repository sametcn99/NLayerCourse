using App.Services.Products;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace App.Services.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductService, ProductService>(); // Add ProductService to the service collection
        services.AddFluentValidationAutoValidation(); // Add FluentValidation auto validation to the service collection
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // Add FluentValidation validators to the service collection
        services.AddAutoMapper(Assembly.GetExecutingAssembly()); // Add AutoMapper to the service collection
        return services;
    }
}

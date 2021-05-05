using Guess.Application.Contracts;
using Guess.Infrastructure.Persistence;
using Guess.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Guess.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IGuessRepository, GuessRepository>();
            services.AddScoped<IGuessContext, GuessContext>();

            return services;
        }
    }
}
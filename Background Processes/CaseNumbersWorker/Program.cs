using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaseNumbersWorker.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CaseNumbersWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    services.AddHostedService<Worker>();
                    services.AddScoped<ICaseNumbersService, TwitterCaseNumbersService>();
                    services.AddHttpClient();

                    services.AddMassTransit(config =>
                    {
                        config.UsingRabbitMq((ctx, cfg) =>
                        {
                            cfg.Host(configuration["EventBusSettings:HostAddress"]);
                        });
                    });
                    services.AddMassTransitHostedService();
                });
    }
}

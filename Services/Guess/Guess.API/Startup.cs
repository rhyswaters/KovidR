using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventBus.Messages.Common;
using Guess.API.Authorization;
using Guess.API.EventBusConsumer;
using Guess.Application;
using Guess.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Guess.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //authorization configuration
            string domain = $"https://{Configuration["Auth0:Domain"]}/";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["Auth0:Audience"];
                // map custom claim added in auth0 rule to User.Identity.Name
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "http://kovidr.ie/userName"
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:guesses", policy => policy.Requirements.Add(new HasPermissionRequirement("read:guesses", domain)));
                options.AddPolicy("write:guesses", policy => policy.Requirements.Add(new HasPermissionRequirement("write:guesses", domain)));
            });

            services.AddSingleton<IAuthorizationHandler, HasPermissionHandler>();

            //dependent projects configuration
            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);

            //service bus configuration
            services.AddMassTransit(config =>
            {
                config.AddConsumer<CaseNumbersPublishedConsumer>();

                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(Configuration["EventBusSettings:HostAddress"]);

                    cfg.ReceiveEndpoint(EventBusConstants.CaseNumbersPublishedQueue, c =>
                    {
                        c.ConfigureConsumer<CaseNumbersPublishedConsumer>(ctx);
                    });
                });
            });
            services.AddMassTransitHostedService();
            services.AddScoped<CaseNumbersPublishedConsumer>();

            //general configuration
            services.AddHealthChecks();
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Guess.API", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Guess.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("default");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}

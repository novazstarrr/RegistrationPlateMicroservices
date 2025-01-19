using MassTransit;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using FluentValidation.AspNetCore;
using AutoMapper;
using Catalog.API.Data.Interface;
using Catalog.API.Data.Repositories;
using Catalog.API.Validators;
using Catalog.API.Middleware;
using Catalog.API.Services;
using Catalog.API.Consumers;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("CatalogConnection"));
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            services.AddValidatorsFromAssemblyContaining<CreatePlateDtoValidator>();
            services.AddScoped<IValidator<CreatePlateDto>, CreatePlateDtoValidator>();

            services.AddScoped<IPlateRepository, PlateRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IPlateService, PlateService>();
            services.AddScoped<ApplicationDbContextSeed>();

            services.AddAutoMapper(typeof(Startup));


            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Plate Registration API",
                    Version = "v1",
                    Description = "API for managing registration plates"
                });
            });


            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });


            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });


            services.AddMassTransit(x =>
            {
                x.AddConsumer<PlateReservationEventConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration["EventBusConnection"], "/", h =>
                    {
                        if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                            h.Username(Configuration["EventBusUserName"]);
                        if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                            h.Password(Configuration["EventBusPassword"]);
                    });

                    cfg.ReceiveEndpoint("plate-reservation-events", e =>
                    {
                        e.ConfigureConsumer<PlateReservationEventConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddMassTransitHostedService();


            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    var pathBase = Configuration["PATH_BASE"];
                    c.SwaggerEndpoint(
                        $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                        "Plate Registration API V1");
                });
            }


            app.UseMiddleware<ErrorHandlingMiddleware>();

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
                await next();
            });

            app.UseForwardedHeaders();
            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/v1/{controller}/{action=Index}/{id?}");
            });
        }
    }
}

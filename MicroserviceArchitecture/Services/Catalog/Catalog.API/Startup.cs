using Catalog.API.Data;
using Catalog.API.Data.Interfaces;
using Catalog.API.Repositories;
using Catalog.API.Repositories.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Catalog.API
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
            /*Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            services.AddHttpClient("basketapi").ConfigureHttpClient(c => c.BaseAddress = new Uri("http://basketapi-service:8008"));
            */

            services.AddScoped<ICatalogContext, CatalogContext>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" });
            });


            //Health check
            services.AddHealthChecks()
                .AddMongoDb(Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);
            
            //Tracing
            services.AddOpenTelemetryTracing(cfg => cfg
            .AddZipkinExporter(o =>
            {
                o.Endpoint = new Uri(Configuration["OpenTelemetryTracingConfiguration:ZipkinUri"]);
                //o.ServiceName = "Catalog Tracing";
            })
            .AddJaegerExporter(c =>
            {
                c.AgentHost = Configuration["OpenTelemetryTracingConfiguration:JaegerHost"];
                c.AgentPort = 6831;
                //c.ServiceName = "Catalog Tracing";
            })
            .AddAspNetCoreInstrumentation()
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                /*endpoints.MapGet("/", async context =>
                {
                    context.Response.Headers.Add("Request-Id", Activity.Current?.TraceId.ToString() ?? string.Empty);

                    using var client = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("basketapi");
                    var content = client.GetStringAsync("/");

                    await context.Response.WriteAsync(await content);
                });*/
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}

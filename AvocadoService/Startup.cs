﻿using AvocadoService.AvocadoServiceDb.DbModels;
using AvocadoService.AvocadoServiceParser;
using AvocadoService.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace AvocadoService
{
    public class Startup
    {
        //#if DEBUG
        //        optionsBuilder.UseNpgsql("Host=roundhouse.proxy.rlwy.net;Port=35316;Username=postgres;Password=IqhHNAjWczuVvNzNbnfdViENrzwdkvyG;Database=railway");
        //#else
        //        optionsBuilder.UseNpgsql("Host=postgres.railway.internal;Port=5432;Username=postgres;Password=IqhHNAjWczuVvNzNbnfdViENrzwdkvyG;Database=railway");
        //#endif 
        //Scaffold-DbContext "Host=roundhouse.proxy.rlwy.net;Port=35316;Username=postgres;Password=IqhHNAjWczuVvNzNbnfdViENrzwdkvyG;Database=railway" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir AvocadoServiceDb/DbModels -f
        public IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc();

            //services.AddLogging();
            services.AddControllers().AddNewtonsoftJson();

            services.AddSwaggerGen();
            services.AddDbContext<railwayContext>();
            services.AddTransient<SubscriptionHelper>();
            services.AddTransient<NotificationHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // app.UseMvcWithDefaultRoute();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V-0.0.1_Release");
                c.RoutePrefix = string.Empty;
            });
            app.UseRouting();
            app.UseCors(builder => builder
      //.WithOrigins("https://elaborate-seahorse-305700.netlify.app")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials()
  );
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
        // This method gets called by the runtime. Use this method to configure endpoints
        public void Endpoints(IEndpointRouteBuilder builder)
        {
        }
    }
}

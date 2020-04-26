using System;
using System.Collections.Generic;
using System.Linq;
// using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using StorageApi.Configuration.Mapping;
using StorageApi.Data;
using StorageApi.Contracts;
using StorageApi.Interfaces.Models;
using StorageApi.Models;

namespace StorageApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        private const string ConnectionStringName = "StorageDbConnection";
        public static string ConnectionString { get; private set; } 

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddOData();
            services.AddControllers().AddNewtonsoftJson();
            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<OutputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }

                foreach (var inputFormatter in options.InputFormatters.OfType<InputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Storage API", Version = "v1" });
            });
            services.AddEntityFrameworkNpgsql().AddDbContext<StorageDbContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString(ConnectionStringName)));
            services.AddAutoMapper(typeof(DefaultProfile));
            services.AddTransient<IArticlesModel, ArticlesModel>();
            services.AddTransient<IStoragesModel, StoragesModel>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConnectionString = Configuration.GetConnectionString(ConnectionStringName);
            
            app.UseSwagger();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Storage API v1");
                c.RoutePrefix = string.Empty;
            });
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().Filter().OrderBy().Count().MaxTop(10);
            });
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<StorageDbContext>();
                try
                {
                    context.Database.Migrate();
                }
                catch{}
            }

        }
    }
}
using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StorageApi.Data;

namespace StorageApi.IntegrationTests.Factories
{
    public class StorageApiWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        private static object _locker = new object();
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");
            
            builder.ConfigureAppConfiguration((context,conf) =>
            {
                conf.AddJsonFile(configPath);
            });
            
            builder.ConfigureServices(services =>
            {
                var config = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .Build();
                services.AddEntityFrameworkNpgsql().AddDbContext<StorageDbContext>(options => 
                    options.UseNpgsql(config.GetConnectionString("StorageDbConnection")));

                var sp = services.BuildServiceProvider();
                lock (_locker)
                {
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var appDb = scopedServices.GetRequiredService<StorageDbContext>();
                        var logger = scopedServices.GetRequiredService<ILogger<StorageApiWebApplicationFactory<Startup>>>();
                        appDb.Database.EnsureDeleted();
                        appDb.Database.EnsureCreated();
                    }
                }
                Thread.Sleep(1000);
            });
        }
    }
}
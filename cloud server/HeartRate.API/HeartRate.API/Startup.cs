using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeartRate.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<BlobManager, BlobManager>();
            services.AddSingleton<CosmosTableManager, CosmosTableManager>();
            services.AddSingleton<StorageTableManager, StorageTableManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            var blobManager = app.ApplicationServices.GetService<BlobManager>();
            var cosmosTableManager = app.ApplicationServices.GetService<CosmosTableManager>();
            cosmosTableManager.Connect();
            var storageTableManager = app.ApplicationServices.GetService<StorageTableManager>();
            storageTableManager.Connect();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
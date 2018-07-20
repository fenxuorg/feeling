using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SmartYi.WebApi.Handlers;
using Swashbuckle.AspNetCore.Swagger;

namespace SmartYi.WebApi
{
    /// <summary>
    /// The start up.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// The constructor of start up.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            AddCores(services);
            AddJsonOptions(services);
            AddApiVersioning(services);
            AddSwaggerGen(services);
            AddAuthentication(services);
            AddAuthorization(services);
            AddApplicationInsightsTelemetry(services);
            AddSingletons(services);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The host environment</param>
        /// <param name="provider">The API version descriptor provider used to enumerate defined API versions.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
                c.DocExpansion(DocExpansion.None);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<GlobalDiagnosticsHandler>();
            }
            app.UseAuthentication();
            app.UseCors("DefaultCORSPolicy");
            app.UseMvc();
        }

        private void AddCores(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                var cors = Configuration["CORS_Default"].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                options.AddPolicy(
                    "DefaultCORSPolicy",
                    builder => builder.WithOrigins(cors)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        private void AddSwaggerGen(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // add a swagger document for each discovered API version
                // note: you might choose to skip or document deprecated API versions differently
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }

                c.OperationFilter<SwaggerDefaultValuesFilter>();

                var basePath = AppContext.BaseDirectory;
                var xmlName = GetType().GetTypeInfo().Module.Name.Replace(".dll", ".xml", StringComparison.OrdinalIgnoreCase).Replace(".exe", ".xml", StringComparison.OrdinalIgnoreCase);
                var xmlPath = Path.Combine(basePath, xmlName);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition(Constants.DefaultSchemeName, new ApiKeyScheme
                {
                    Description = Constants.ApiKeySchemeDescription,
                    Name = Constants.ApiKeySchemeName,
                    In = "header",
                    Type = "apiKey",
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { Constants.DefaultSchemeName, Array.Empty<string>() } });
                c.DescribeAllEnumsAsStrings();
                c.DescribeStringEnumsInCamelCase();
            });
        }

        private static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"OPS NotificationService Web API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "OPS NotificationService Web API",
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

        private void AddApiVersioning(IServiceCollection services)
        {
            services.AddMvcCore().AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            services.AddApiVersioning(o => o.ReportApiVersions = true);
        }

        private void AddJsonOptions(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = Constants.DefaultSchemeName;
            }).AddScheme<TokenAuthenticationOptions, TokenAuthenticationHandler>(Constants.DefaultSchemeName, o => { });
        }

        private void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Member", policy => policy.RequireClaim("token"));
            });
        }

        private void AddApplicationInsightsTelemetry(IServiceCollection services)
        {
            var aiOptions = new ApplicationInsightsServiceOptions
            {
                EnableAdaptiveSampling = false,
                InstrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"],
            };
            services.AddApplicationInsightsTelemetry(aiOptions);
        }

        private void AddSingletons(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
        }
    }
}

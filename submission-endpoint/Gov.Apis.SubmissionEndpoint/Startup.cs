using Gov.Apis.SubmissionEndpoint.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Octokit;
using System;

namespace Gov.Apis.SubmissionEndpoint
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
            services.AddMemoryCache();

            services.AddScoped<IGitHubClient>(s => new GitHubClient(
                productInformation: new ProductHeaderValue(Configuration["GITHUB_USER"])) {
                    Credentials = new Credentials(Configuration["GITHUB_USER"], Configuration["GITHUB_ACCESS_TOKEN"]),
                });

            services.AddScoped<ISubmissionService>(s => new GitHubSubmissionService(
                logger: s.GetRequiredService<ILogger<GitHubSubmissionService>>(),
                gitHubClient: s.GetRequiredService<IGitHubClient>(),
                cache: s.GetRequiredService<IMemoryCache>(),
                repositoryOwner: Configuration["REPOSITORY_OWNER"] ?? "usds",
                repositoryName: "apis.gov",
                apisDotJsonPath: "docs/apis.json"));

            services.AddCors(options => {
                options.AddDefaultPolicy(builder => {
                    builder.WithOrigins("https://usds.github.io")
                        .WithHeaders(HeaderNames.ContentType)
                        .SetPreflightMaxAge(TimeSpan.FromMinutes(15));
                });
            });

            services.AddControllers(options => {
                options.ReturnHttpNotAcceptable = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

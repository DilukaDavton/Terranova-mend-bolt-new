using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Terranova_APIClient;
using Terranova_APIClient.Models;
using Terranova_DBClient;
using Terranova_GraphClient;
using Terranova_GraphClient.Settings;

namespace Terranova_WebAPI
{
    public class Startup
    {
        private const string WellKnownURL = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";
        private const string AllowAnyOrigins = "AllowAnyOrigin";

        public static string Token = "";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var clientId = Configuration.GetSection("AzureAd:ClientId").Value;
            var domainDev = Configuration.GetSection("AzureAd:DomainDev").Value;
            var domainProd = Configuration.GetSection("AzureAd:DomainProd").Value;
            Token = Configuration.GetSection("Token").Value;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,//not allow to validate the issuer of the token since this is multitetnant app
                            ValidateAudience = true,//allow to validate audience
                            ValidAudiences = new[]
                            { 
								// accepted token version is v2 (accessTokenAcceptedVersion=2 in manifest file in Azure App)
								clientId,
								// accepted token version is v1 (accessTokenAcceptedVersion=1 or null in manifest file in Azure App) for more https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-protected-web-api-app-registration#accepted-token-version
								$"api://{domainDev}/{clientId}",
                                $"api://{domainProd}/{clientId}",
                            },
                            SaveSigninToken = true//allow to save raw bootstrap token from the office application
                        };
                        //retrieve the configuration from metadata available in wellknownurl
                        options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                            WellKnownURL,
                            new OpenIdConnectConfigurationRetriever());
                    });
            services.AddCors(c =>
            {
                c.AddPolicy(AllowAnyOrigins, options => options.AllowAnyOrigin().AllowAnyHeader());
            });

            var infos = Configuration.GetSection("ReportApiInfo")
                .GetChildren()
                .ToDictionary(x => int.Parse(x.Key), y => y.Get<ReportApiInfo>());

            services.Configure<OBOSettings>(Configuration.GetSection("AzureAd"));
            services.AddSingleton(infos);
            services.AddScoped<IOnBehalfOfUserFactory, OnBehalfOfUserFactory>();
            services.AddScoped<IAPIClient, TerranovaAPIClient>();
            services.AddScoped<IDBClient, AzureDBClient>();
            services.AddControllersWithViews();
            services.AddHttpClient<GraphServiceClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append(
                         "Cache-Control", $"no-cache, max-age=0");
                }
            });
            app.UseCors(AllowAnyOrigins);
            app.UseRouting();
            
            app.UseAuthorization();
        
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

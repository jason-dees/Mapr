using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MapR.Api.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using MapR.Api.Hubs;
using MapR.DataStores.Configuration;

namespace MapR.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var corsOrigins = Configuration["CORS"].Split(";");
            services.AddCors(options => options.AddDefaultPolicy(
                builder => {
                    builder
                        .WithOrigins(corsOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                })
            );

            services.AddMvc(options =>
            {
                options.Filters.Add(new MapRExceptionFilter());
            });
            
            services.AddSignalR();

            ServiceRegistrator.AddAzureTableAndBlobStorage(services,
                Configuration["MapR:CosmosConnectionString"],
                Configuration["MapR:BlobStorageConnectionString"]);
            //CosmosConfiguration.Register(services, Configuration);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Google:ClientSecret"];
                    googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MapR Api", Version = "v1" });
            });

    }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {


            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MapHub>("/maphub");

            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MapR Api V1");
            });
        }
    }
}

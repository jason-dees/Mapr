using System.Threading;
using System.Threading.Tasks;
using MapR.Stores.Game;
using MapR.Hubs;
using MapR.Stores.Identity.Models;
using MapR.Stores.Identity.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MapR.Map;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using MapR.Stores.Marker;

namespace MapR
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc()
                .AddFeatureFolders()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR();

			services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddDefaultTokenProviders();

			services.AddAuthentication()
				.AddGoogle(googleOptions => {
					googleOptions.ClientId = Configuration["Google:ClientId"];
					googleOptions.ClientSecret = Configuration["Google:ClientSecret"];
				});

			services.Configure<RazorViewEngineOptions>(o => {
                o.ViewLocationFormats.Clear();
                o.ViewLocationFormats.Add("/Features/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("/Features/Shared/{0}" + RazorViewEngine.ViewExtension);
            });

            AddAzureCloudStuff(services, Configuration["MapR:ConnectionString"]);
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
                routes.MapHub<MapHub>("/mapHub");
            });
            app.UseMvc();
        }
    
        static void AddAzureCloudStuff(IServiceCollection services, string connectionString) {
            var account = CloudStorageAccount.Parse(connectionString);

            services.AddSingleton((sp) => account);
            services.AddSingleton<CloudTableClient>((sp) => account.CreateCloudTableClient());
            services.AddSingleton<CloudBlobClient>((sp) => account.CreateCloudBlobClient());

            services.AddStartupTask<CloudInitialize>();

            services.AddSingleton<IStoreGames>((serviceProvider) => {
                var cloudClient = serviceProvider.GetService(typeof(CloudTableClient)) as CloudTableClient;
                return new GameStore(cloudClient.GetTableReference("games"));
            });
            services.AddSingleton<IStoreMaps>((serviceProvider) => {    
                var cloudClient = serviceProvider.GetService(typeof(CloudTableClient)) as CloudTableClient;
                var blobClient = serviceProvider.GetService(typeof(CloudBlobClient)) as CloudBlobClient; ;

                return new MapStore(cloudClient.GetTableReference("gamemaps"),
                    blobClient.GetContainerReference("mapimagestorage"));
            });

			services.AddSingleton<IStoreMarkers>((sreviceProvider) => {
				return new MarkerStore();
			});
        }
    }

    //Shamelessly stolen from https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-2/
    public interface IStartupTask {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
            where T : class, IStartupTask
            => services.AddTransient<IStartupTask, T>();
    }

    public static class StartupTaskWebHostExtensions {
        public static async Task RunWithTasksAsync(this IWebHost webHost, CancellationToken cancellationToken = default) {
            // Load all tasks from DI
            var startupTasks = webHost.Services.GetServices<IStartupTask>();

            // Execute all the tasks
            foreach (var startupTask in startupTasks) {
                await startupTask.ExecuteAsync(cancellationToken);
            }

            // Start the tasks as normal
            await webHost.RunAsync(cancellationToken);
        }
    }

    class CloudInitialize : IStartupTask {

        readonly string[] _tables;
        readonly string[] _blobContainers;
        readonly CloudTableClient _tableClient;
        readonly CloudBlobClient _blobClient;
        public CloudInitialize(CloudStorageAccount account) {
            _tableClient = account.CreateCloudTableClient();
            _blobClient = account.CreateCloudBlobClient();
            _tables = new string[] {
                "users",
                "roles",
                "games",
                "gameplayers",
                "gamemasters",
                "mapmarkers",
                "gamemaps"
            };
            _blobContainers = new string[] {
                "mapimagestorage"
            };
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default) {
            foreach(var table in _tables) {
                CloudTable cloudTable = _tableClient.GetTableReference(table);
                await cloudTable.CreateIfNotExistsAsync();
            }
            foreach(var container in _blobContainers) {
                var storage = _blobClient.GetContainerReference(container);
                await storage.CreateIfNotExistsAsync();
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MapR.Data.Models;
using MapR.DataStores.Configuration;
using MapR.DataStores.Stores;
using MapR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace MapR {
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

			services.AddIdentity<MapRUser, MapRRole>()
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

            ServiceRegistrator.AddAzureCloudStuff(services, Configuration["MapR:ConnectionString"]);
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
                routes.MapHub<MapHub>("/mapHub");
            });
            app.UseMvc();
            if (env.IsDevelopment()) {
                RunNpmScript("watch_webapp");
            }
        }

        //Stole from https://github.com/EEParker/aspnetcore-vueclimiddleware/blob/master/src/VueCliMiddleware/Util/ScriptRunner.cs
        //But without the sweet logging
        void RunNpmScript(string scriptName) {
            var npmExe = "npm";
            var completeArguments = $"run  {scriptName}";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                completeArguments = $"/c {npmExe} {completeArguments}";
                npmExe = "cmd";
            }

            var processStartInfo = new ProcessStartInfo(npmExe) {
                Arguments = completeArguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = ""
            };

            LaunchNodeProcess(processStartInfo);
        }

        static Process LaunchNodeProcess(ProcessStartInfo startInfo) {
            try {
                var process = Process.Start(startInfo);
                process.EnableRaisingEvents = true;

                return process;
            }
            catch (Exception ex) {
                var message = $"Failed to start '{startInfo.FileName}'. To resolve this:.\n\n"
                            + $"[1] Ensure that '{startInfo.FileName}' is installed and can be found in one of the PATH directories.\n"
                            + $"    Current PATH enviroment variable is: { Environment.GetEnvironmentVariable("PATH") }\n"
                            + "    Make sure the executable is in one of those directories, or update your PATH.\n\n"
                            + "[2] See the InnerException for further details of the cause.";
                throw new InvalidOperationException(message, ex);
            }
        }
    }
}

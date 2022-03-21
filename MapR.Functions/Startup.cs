using System;
using MapR.DataStores.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(MapR.Functions.Startup))]

namespace MapR.Functions {
    public class Startup : FunctionsStartup {
        public override void Configure(IFunctionsHostBuilder builder) {

            ServiceRegistrator.AddAzureTableAndBlobStorage(builder.Services,
                Environment.GetEnvironmentVariable("MapR:TableStorageConnectionString"),
                Environment.GetEnvironmentVariable("MapR:BlobStorageConnectionString"));
        }
    }
}
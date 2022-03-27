using System;
using MapR.DataStores.Configuration;
using MapR.Functions.Initialization;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MapR.Functions.Initialization {

    public class Startup : FunctionsStartup {

        public override void Configure(IFunctionsHostBuilder builder) {
            var config = builder.GetContext().Configuration;
            var section = config.GetSection("MapR");
            string tableStorage = section.GetValue<string>("TableStorageConnectionString");
            string blobStorage = section.GetValue<string>("BlobStorageConnectionString");
            ServiceRegistrator.AddAzureTableAndBlobStorage(builder.Services,
                tableStorage,
                blobStorage);

        }

    }

}

public class Configuration {
    public string TableStorageConnectionString { get; set; }
    public string BlobStorageConnectionString { get; set; }
}
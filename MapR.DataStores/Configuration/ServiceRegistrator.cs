using System.Threading;
using System.Threading.Tasks;
using MapR.Data.Extensions;
using MapR.Data.Stores;
using MapR.DataStores.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Configuration {
    public static class ServiceRegistrator {
        public static void AddAzureTableAndBlobStorage(IServiceCollection services, string connectionString) {
			var mapper = AutoMapperConfiguration.MapperConfiguration(services);
			var account = CloudStorageAccount.Parse(connectionString);
            var cloudTableClient = account.CreateCloudTableClient();
            var cloudBlobClient = account.CreateCloudBlobClient();

            services.AddSingleton((sp) => account);
            services.AddSingleton((sp) => cloudTableClient);
            services.AddSingleton((sp) => cloudBlobClient);
            services.AddStartupTask<CloudInitialize>();

            services.AddSingleton<IStoreGames>((serviceProvider) => 
                new GameStore(cloudTableClient.GetTableReference("games"), 
                    mapper));

            services.AddSingleton<IStoreMaps>((serviceProvider) => 
                new MapStore(cloudTableClient.GetTableReference("gamemaps"),
                    cloudBlobClient.GetContainerReference("mapimagestorage"),
					mapper));

            services.AddSingleton<IStoreMarkers>((sp) => 
                new MarkerStore(cloudTableClient.GetTableReference("mapmarkers"),
                    cloudBlobClient.GetContainerReference("markericonstorage"), 
                    mapper));
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
                "mapimagestorage",
                "markericonstorage"
            };
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default) {
            foreach (var table in _tables) {
                CloudTable cloudTable = _tableClient.GetTableReference(table);
                await cloudTable.CreateIfNotExistsAsync();
            }
            foreach (var container in _blobContainers) {
                var storage = _blobClient.GetContainerReference(container);
                await storage.CreateIfNotExistsAsync();
            }
        }
    }
}


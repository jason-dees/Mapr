using System;
using System.Threading;
using System.Threading.Tasks;
using MapR.Data.Extensions;
using MapR.Data.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Configuration {
    public static class ServiceRegistrator {
        public static void AddAzureCloudStuff(IServiceCollection services, string connectionString) {
			var mapper = AutoMapperConfiguration.MapperConfiguration(services);
			var account = CloudStorageAccount.Parse(connectionString);

            services.AddSingleton((sp) => account);
            services.AddSingleton<CloudTableClient>((sp) => account.CreateCloudTableClient());
            services.AddSingleton<CloudBlobClient>((sp) => account.CreateCloudBlobClient());

            services.AddStartupTask<CloudInitialize>();

            services.AddSingleton<IStoreGames>((serviceProvider) => {
                var cloudClient = serviceProvider.GetService(typeof(CloudTableClient)) as CloudTableClient;
                return new GameStore(cloudClient.GetTableReference("games"), mapper);
            });
            services.AddSingleton<IStoreMaps>((serviceProvider) => {
                var cloudClient = serviceProvider.GetService(typeof(CloudTableClient)) as CloudTableClient;
                var blobClient = serviceProvider.GetService(typeof(CloudBlobClient)) as CloudBlobClient;

				return new MapStore(cloudClient.GetTableReference("gamemaps"),
                    blobClient.GetContainerReference("mapimagestorage"),
					mapper);
            });
            services.AddSingleton<IStoreMarkers>((sp) => new MarkerStore(mapper));
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


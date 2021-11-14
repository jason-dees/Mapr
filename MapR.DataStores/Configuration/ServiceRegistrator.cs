using Azure.Storage.Blobs;
using MapR.Data.Extensions;
using MapR.Data.Stores;
using MapR.DataStores.Models;
using MapR.DataStores.Stores;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace MapR.DataStores.Configuration
{
    public static class ServiceRegistrator
    {
        public static async void AddAzureTableAndBlobStorage(IServiceCollection services, string tableConnectionString, string blobConnectionSting)
        {
            var mapper = AutoMapperConfiguration.MapperConfiguration(services);
            var account = CloudStorageAccount.Parse(tableConnectionString);
            var cloudTableClient = account.CreateCloudTableClient();
            var cloudBlobClient = new BlobServiceClient(blobConnectionSting);

            services.AddSingleton((sp) => cloudTableClient);
            services.AddSingleton((sp) => cloudBlobClient);
            services.AddStartupTask<CloudInitialize>();

            services.AddSingleton<IStoreGames>((serviceProvider) =>
            {
                var tableClient = cloudTableClient.GetTableReference("games");
                var tableAccess = new CloudTableAccess<GameModel>(tableClient);
                return new GameStore(tableAccess, mapper);
            });

            var mapBlobAccess = await cloudBlobClient.CreateBlobContainerAsync("mapimagestorage");
            services.AddSingleton<IStoreMaps>((serviceProvider) =>
            {
                var tableClient = cloudTableClient.GetTableReference("gamemaps");
                var tableAccess = new CloudTableAccess<MapModel>(tableClient);
                var blobAccces = new CloudBlobAccess(mapBlobAccess);
                return new MapStore(tableAccess,
                    blobAccces,
                    mapper);
            });

            var markerBlobAccess = await cloudBlobClient.CreateBlobContainerAsync("markericonstorage");
            services.AddSingleton<IStoreMarkers>((sp) =>
            {
                var tableClient = cloudTableClient.GetTableReference("mapmarkers");
                var tableAccess = new CloudTableAccess<MarkerModel>(tableClient);
                var blobAccces = new CloudBlobAccess(markerBlobAccess);
                return new MarkerStore(tableAccess,
                    blobAccces,
                    mapper);
            });
        }
    }

    class CloudInitialize : IStartupTask
    {

        readonly string[] _tables;
        readonly string[] _blobContainers;
        readonly CloudTableClient _tableClient;
        readonly BlobServiceClient _blobClient;

        public CloudInitialize(CloudTableClient tableClient, BlobServiceClient blobClient)
        {
            _tableClient = tableClient;
            _blobClient = blobClient;
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

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            foreach (var table in _tables)
            {
                CloudTable cloudTable = _tableClient.GetTableReference(table);
                await cloudTable.CreateIfNotExistsAsync();
            }
            foreach (var container in _blobContainers)
            {
                var storage = _blobClient.GetBlobContainerClient(container);
                await storage.CreateIfNotExistsAsync();
            }
        }
    }
}


using MapR.CosmosStores.Stores;
using MapR.Data.Extensions;
using MapR.Data.Stores;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MapR.CosmosStores {
    public static class CosmosConfiguration {
        public static void Register(IServiceCollection services, IConfiguration configuration) {
            var connectionString = configuration["MapR:ConnectionString"];
            var databaseId = configuration["MapR:DatabaseId"];
            var containerId = configuration["MapR:ContainerId"];
            var paritionKey = configuration["MapR:ParitionKey"];

            CosmosClient client = new CosmosClient(connectionString: connectionString);
            var cosmosTask = client.CreateDatabaseIfNotExistsAsync(databaseId);
            cosmosTask.Wait();

            var containerTask = client.GetDatabase(databaseId).CreateContainerIfNotExistsAsync(containerId, $"/{paritionKey}");
            containerTask.Wait();
            Container container = containerTask.Result;

            services.AddSingleton(_ => client);
            services.AddSingleton(_ => container);

            var mapper = AutoMapperConfiguration.MapperConfiguration(services);

            services.AddSingleton<IStoreGames>(_ =>
               new GameStore(_.GetService<Container>(),
                   mapper));
            AddAzureBlobStorage(services, configuration);
            services.AddSingleton<IStoreMaps>(_ => 
                new MapStore(_.GetService<IStoreGames>(),
                _.GetService<CloudBlobContainer>())
            );

        }

        static void AddAzureBlobStorage(IServiceCollection services, IConfiguration configuration) {
            var connectionString = configuration["MapR:BlobStorageConnectionString"];
            var account = CloudStorageAccount.Parse(connectionString);
            var cloudBlobClient = account.CreateCloudBlobClient();
            
            services.AddSingleton(_ => account);
            services.AddSingleton(_ => cloudBlobClient);
            services.AddSingleton(_ => cloudBlobClient.GetContainerReference("mapimagestorage"));
            services.AddSingleton(_ => cloudBlobClient.GetContainerReference("markericonstorage"));

            services.AddStartupTask<CloudInitialize>();
            
        }
    }

    class CloudInitialize : IStartupTask {

        readonly string[] _blobContainers;
        readonly CloudBlobClient _blobClient;

        public CloudInitialize(CloudStorageAccount account) {
            _blobClient = account.CreateCloudBlobClient();
            _blobContainers = new string[] {
                "mapimagestorage",
                "markericonstorage"
            };
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default) {
            foreach (var container in _blobContainers) {
                var storage = _blobClient.GetContainerReference(container);
                try {
                    var c = await storage.CreateIfNotExistsAsync();
                }
                catch(Exception e) {

                }
            }
        }
    }
}

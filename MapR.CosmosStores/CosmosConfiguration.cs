using MapR.CosmosStores.Stores;
using MapR.CosmosStores.Stores.Internal;
using MapR.Data.Extensions;
using MapR.Data.Models;
using MapR.Data.Stores;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;

namespace MapR.CosmosStores {
    public static class CosmosConfiguration {
        public static void Register(IServiceCollection services, IConfiguration configuration) {
            services.AddIdentity<MapRUser, MapRRole>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddDefaultTokenProviders();

            var connectionString = configuration["MapR:CosmosConnectionString"];
            var databaseId = configuration["MapR:DatabaseId"];
            var gameContainerId = configuration["MapR:GameContainerId"];
            var userContainerId = configuration["MapR:UserContainerId"];
            var gameParitionKey = configuration["MapR:GamePartitionKey"];
            var userParitionKey = configuration["MapR:UserPartitionKey"];

            CosmosClient client = new CosmosClient(connectionString: connectionString);
            var cosmosTask = client.CreateDatabaseIfNotExistsAsync(databaseId);
            cosmosTask.Wait();

            var gameContainerTask = client.GetDatabase(databaseId).CreateContainerIfNotExistsAsync(gameContainerId, $"/{gameParitionKey}");
            gameContainerTask.Wait();
            Container gameContainer = gameContainerTask.Result;

            var userContainerTask = client.GetDatabase(databaseId).CreateContainerIfNotExistsAsync(userContainerId, $"/{userParitionKey}");
            userContainerTask.Wait();
            Container userContainer = userContainerTask.Result;

            services.AddSingleton(_ => client);
            services.AddSingleton(_ => userContainer);

            var mapper = AutoMapperConfiguration.MapperConfiguration(services);
            services.AddSingleton<IAmAGameContainerHelper>(_ => new GameContainerHelper(gameContainer));

            services.AddSingleton<IStoreGames, GameStore>();

            AddAzureBlobStorage(services, configuration);
            services.AddSingleton<IStoreMaps>(_ => 
                new MapStore(_.GetService<IAmAGameContainerHelper>(),
                    new ImageStore(_.GetService<BlobContainerClient>()),
                    mapper)
            );

        }

        static void AddAzureBlobStorage(IServiceCollection services, IConfiguration configuration) {
            var connectionString = configuration["MapR:BlobStorageConnectionString"];
            var cloudBlobClient = new BlobContainerClient(connectionString, "mapimagestorage");
            services.AddSingleton(_ => cloudBlobClient);

            services.AddStartupTask<CloudInitialize>();
        }
    }

    class CloudInitialize : IStartupTask {

        readonly string[] _blobContainers;
        readonly BlobContainerClient _blobClient;
        readonly string _connectionString;

        public CloudInitialize(string connectionString) {
            _connectionString = connectionString;
            _blobContainers = new string[] {
                "mapimagestorage",
                "markericonstorage"
            };
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default) {
            foreach (var container in _blobContainers) {
                var cloudBlobClient = new BlobContainerClient(_connectionString, container);
                _ = await cloudBlobClient.CreateIfNotExistsAsync(cancellationToken: CancellationToken.None);
            }
        }
    }
}

using MapR.CosmosStores.Stores;
using MapR.Data.Stores;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        }
    }
}

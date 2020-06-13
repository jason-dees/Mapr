using MapR.CosmosStores.Stores.Internal;
using MapR.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoreMapRUser = MapR.CosmosStores.Models.MapRUser;

namespace MapR.CosmosStores.Stores {
    public class UserStore : IUserStore<Data.Models.MapRUser> {
        readonly Container _userContainer;

        public UserStore(Container userContainer) {
            _userContainer = userContainer;
        }
        public async Task<IdentityResult> CreateAsync(MapRUser user, CancellationToken cancellationToken) {
            var storeUser = new StoreMapRUser { 
                LoginProvider = user.LoginProvider,
                ProviderKey = user.ProviderKey,
                NameIdentifier = user.NameIdentifier
            };

            ItemResponse<StoreMapRUser> response = await _userContainer.CreateItemAsync<StoreMapRUser>(storeUser);
            if (response.StatusCode == System.Net.HttpStatusCode.Created) {
                return new MapRIdentityResult(true);
            }
            return new MapRIdentityResult(false);
        }

        public Task<IdentityResult> DeleteAsync(MapRUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public void Dispose() {
        }

        public async Task<MapRUser> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            var sql = $"SELECT * from users as u where u.id = \"{userId}\"";
            return await GetSingleUserByQuery(sql);
        }

        public async Task<MapRUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            var sql = $"SELECT * from users as u where u.id = \"{normalizedUserName}\"";
            return await GetSingleUserByQuery(sql);
        }

        public Task<string> GetNormalizedUserNameAsync(MapRUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserIdAsync(MapRUser user, CancellationToken cancellationToken) {
            return user.NameIdentifier;
        }

        public async Task<string> GetUserNameAsync(MapRUser user, CancellationToken cancellationToken) {
            return user.NameIdentifier;
        }

        public async Task SetNormalizedUserNameAsync(MapRUser user, string normalizedName, CancellationToken cancellationToken) {
            
        }

        public async Task SetUserNameAsync(MapRUser user, string userName, CancellationToken cancellationToken) {
           
        }

        public async Task<IdentityResult> UpdateAsync(MapRUser user, CancellationToken cancellationToken) {
            var storeUser = new StoreMapRUser {
                LoginProvider = user.LoginProvider,
                ProviderKey = user.ProviderKey,
                NameIdentifier = user.NameIdentifier
            };
            try {
                await _userContainer.UpsertItemAsync<StoreMapRUser>(storeUser);
            }
            catch (CosmosException) {
                return new MapRIdentityResult(false);
            }
            return new MapRIdentityResult(true);
        }

        async Task<MapRUser> GetSingleUserByQuery(string query) {
            var queryDefinition = new QueryDefinition(query);
            var feedIterator = _userContainer.GetItemQueryIterator<StoreMapRUser>(queryDefinition);
            if (feedIterator.HasMoreResults) {
                var dataUser = (await feedIterator.ReadNextAsync()).FirstOrDefault();
                if (dataUser != null) {
                    return new MapRUser {
                        LoginProvider = dataUser.LoginProvider,
                        ProviderKey = dataUser.ProviderKey,
                        NameIdentifier = dataUser.NameIdentifier
                    };
                }
            }
            return null;

        }
    }

}

class MapRIdentityResult : IdentityResult {
    public MapRIdentityResult(bool succeeded) {
        base.Succeeded = succeeded;
    }
}
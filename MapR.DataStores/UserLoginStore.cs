using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MapR.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores {
    public partial class UserStore : IUserLoginStore<MapRUser> {

        public Task AddLoginAsync(MapRUser user, UserLoginInfo login, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<MapRUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            var providerKeyQuery = TableQuery.GenerateFilterCondition("ProviderKey", QueryComparisons.Equal, providerKey);
            var loginProviderQuery = TableQuery.GenerateFilterCondition("LoginProvider", QueryComparisons.Equal, loginProvider);
            var query = new TableQuery<MapRUser>()
                .Where(TableQuery.CombineFilters(providerKeyQuery,
                TableOperators.And,
                loginProviderQuery));
            return (await _userTable.ExecuteQuerySegmentedAsync<MapRUser>(query, null)).Results.FirstOrDefault();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(MapRUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(MapRUser user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MapR.DataStores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Stores {
    public partial class UserStore : IUserLoginStore<Data.Models.MapRUser> {

        public Task AddLoginAsync(Data.Models.MapRUser user, UserLoginInfo login, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<Data.Models.MapRUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            var providerKeyQuery = TableQuery.GenerateFilterCondition("ProviderKey", QueryComparisons.Equal, providerKey);
            var loginProviderQuery = TableQuery.GenerateFilterCondition("LoginProvider", QueryComparisons.Equal, loginProvider);
            var query = new TableQuery<MapRUser>()
                .Where(TableQuery.CombineFilters(providerKeyQuery,
                TableOperators.And,
                loginProviderQuery));
            return (await _userTable.ExecuteQuerySegmentedAsync<MapRUser>(query, null)).Results.FirstOrDefault();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(Data.Models.MapRUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(Data.Models.MapRUser user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}

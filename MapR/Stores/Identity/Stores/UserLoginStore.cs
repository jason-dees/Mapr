using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MapR.Stores.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Stores.Identity.Stores {
    public partial class UserStore : IUserLoginStore<ApplicationUser> {

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            var providerKeyQuery = TableQuery.GenerateFilterCondition("ProviderKey", QueryComparisons.Equal, providerKey);
            var loginProviderQuery = TableQuery.GenerateFilterCondition("LoginProvider", QueryComparisons.Equal, loginProvider);
            var query = new TableQuery<ApplicationUser>()
                .Where(TableQuery.CombineFilters(providerKeyQuery,
                TableOperators.And,
                loginProviderQuery));
            return (await _userTable.ExecuteQuerySegmentedAsync<ApplicationUser>(query, null)).Results.FirstOrDefault();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}

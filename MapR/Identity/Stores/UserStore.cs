using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MapR.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Identity.Stores {
	public partial class UserStore : IUserStore<ApplicationUser> {
		const string _partition = "MapR";
		readonly CloudTable _userTable;
		public UserStore(CloudTableClient tableClient) {
			_userTable = tableClient.GetTableReference("users");
		}

		public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken) {
			user.PartitionKey = _partition;
            user.RowKey = user.NameIdentifier;
			TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(user);

			await _userTable.ExecuteAsync(insertOrMergeOperation);

			return new MapRIdentityResult();
		}

		public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
		}

		public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            var idQuery = TableQuery.GenerateFilterCondition("NameIdentifier", QueryComparisons.Equal, userId);
            var query = new TableQuery<ApplicationUser>()
                .Where(idQuery);
            return (await _userTable.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
        }

		public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            var providerKeyQuery = TableQuery.GenerateFilterCondition("UserName", QueryComparisons.Equal, normalizedUserName);
            var query = new TableQuery<ApplicationUser>()
                .Where(providerKeyQuery); 
            return (await _userTable.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
        }

		public async Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) {
			return user.Email;
		}

		public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) {
			return user.Email;
		}

		public async Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) {
			return user.UserName;
		}

		public async Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken) {
			
		}

		public async Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken) {
			
		}

		public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken) {
			TableOperation operation = TableOperation.Replace(user);

			await _userTable.ExecuteAsync(operation);

			return new MapRIdentityResult();
		}
	}
}

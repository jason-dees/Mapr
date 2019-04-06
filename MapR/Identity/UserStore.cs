using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Identity {
	public class UserStore : IUserStore<ApplicationUser> {
		const string _partition = "MapR";
		readonly CloudTable _userTable;
		public UserStore(CloudTableClient tableClient) {
			_userTable = tableClient.GetTableReference("users");
		}

		public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken) {
			user.PartitionKey = _partition;
			user.RowKey = user.Email;
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
			TableOperation operation = TableOperation.Retrieve<ApplicationUser>(_partition, userId);
			var result = await _userTable.ExecuteAsync(operation);
			return result.Result as ApplicationUser;
		}

		public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public async Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) {
			return user.Email;
		}

		public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) {
			return user.Email;
		}

		public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
	}
}

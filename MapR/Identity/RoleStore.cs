using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Identity {
	public class RoleStore : IRoleStore<ApplicationRole> {
		const string _partition = "MapR";
		readonly CloudTable _roleTable;
		public RoleStore(CloudTableClient tableClient) {
			_roleTable = tableClient.GetTableReference("roles");
		}

		public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken) {
			role.PartitionKey = _partition;
			role.RowKey = role.Id;
			TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(role);

			await _roleTable.ExecuteAsync(insertOrMergeOperation);

			return new MapRIdentityResult();
		}

		public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
		}

		public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
			TableOperation operation = TableOperation.Retrieve<ApplicationRole>(_partition, roleId);
			var result = await _roleTable.ExecuteAsync(operation);
			return result.Result as ApplicationRole;
		}

		public Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken) {
			TableOperation operation = TableOperation.Replace(role);

			await _roleTable.ExecuteAsync(operation);

			return new MapRIdentityResult();
		}
	}
}
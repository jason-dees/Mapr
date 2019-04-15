using System;
using System.Threading;
using System.Threading.Tasks;
using MapR.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores {
	public class RoleStore : IRoleStore<MapRRole> {
		const string _partition = "MapR";
		readonly CloudTable _roleTable;
		public RoleStore(CloudTableClient tableClient) {
			_roleTable = tableClient.GetTableReference("roles");
		}

		public async Task<IdentityResult> CreateAsync(MapRRole role, CancellationToken cancellationToken) {
			role.PartitionKey = _partition;
			role.RowKey = role.Id;
			TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(role);

			await _roleTable.ExecuteAsync(insertOrMergeOperation);

			return new MapRIdentityResult();
		}

		public Task<IdentityResult> DeleteAsync(MapRRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
		}

		public async Task<MapRRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
			TableOperation operation = TableOperation.Retrieve<MapRRole>(_partition, roleId);
			var result = await _roleTable.ExecuteAsync(operation);
			return result.Result as MapRRole;
		}

		public Task<MapRRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task<string> GetNormalizedRoleNameAsync(MapRRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task<string> GetRoleIdAsync(MapRRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task<string> GetRoleNameAsync(MapRRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task SetNormalizedRoleNameAsync(MapRRole role, string normalizedName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task SetRoleNameAsync(MapRRole role, string roleName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public async Task<IdentityResult> UpdateAsync(MapRRole role, CancellationToken cancellationToken) {
			TableOperation operation = TableOperation.Replace(role);

			await _roleTable.ExecuteAsync(operation);

			return new MapRIdentityResult();
		}
	}
}
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MapR.DataStores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Stores {
	public class RoleStore : IRoleStore<Data.Models.MapRRole> {
		const string _partition = "MapR";
		readonly CloudTable _roleTable;
		readonly IMapper _mapper;
		public RoleStore(CloudTableClient tableClient, IMapper mapper) {
			_roleTable = tableClient.GetTableReference("roles");
			_mapper = mapper;
		}

		public async Task<IdentityResult> CreateAsync(Data.Models.MapRRole roleModel, CancellationToken cancellationToken) {
			var role = _mapper.Map<MapRRole>(roleModel);
			role.PartitionKey = _partition;
			role.RowKey = role.Id;
			TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(role);

			await _roleTable.ExecuteAsync(insertOrMergeOperation);

			return new MapRIdentityResult();
		}

		public Task<IdentityResult> DeleteAsync(Data.Models.MapRRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
		}

		public async Task<Data.Models.MapRRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
			TableOperation operation = TableOperation.Retrieve<MapRRole>(_partition, roleId);
			var result = await _roleTable.ExecuteAsync(operation);
			return result.Result as MapRRole;
		}

		public Task<Data.Models.MapRRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task<string> GetNormalizedRoleNameAsync(Data.Models.MapRRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task<string> GetRoleIdAsync(Data.Models.MapRRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task<string> GetRoleNameAsync(Data.Models.MapRRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task SetNormalizedRoleNameAsync(Data.Models.MapRRole role, string normalizedName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public Task SetRoleNameAsync(Data.Models.MapRRole role, string roleName, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public async Task<IdentityResult> UpdateAsync(Data.Models.MapRRole roleModel, CancellationToken cancellationToken) {
			var role = _mapper.Map<MapRRole>(roleModel);
			TableOperation operation = TableOperation.Replace(role);

			await _roleTable.ExecuteAsync(operation);

			return new MapRIdentityResult();
		}
	}
}
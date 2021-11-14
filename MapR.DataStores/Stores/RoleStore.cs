using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MapR.DataStores.Models;
using Microsoft.AspNetCore.Identity;

namespace MapR.DataStores.Stores {
	public class RoleStore : IRoleStore<Data.Models.MapRRole> {
		const string _partition = "MapR";
        readonly IAccessCloudTableData<MapRRole> _cloudTableAccess;
		readonly IMapper _mapper;
		public RoleStore(IAccessCloudTableData<MapRRole> cloudTableAccess, IMapper mapper) {
			//_roleTable = tableClient.GetTableReference("roles");
			_cloudTableAccess = cloudTableAccess;
			_mapper = mapper;
		}

		public async Task<IdentityResult> CreateAsync(Data.Models.MapRRole roleModel, CancellationToken cancellationToken) {
			var role = _mapper.Map<MapRRole>(roleModel);
			role.PartitionKey = _partition;
			role.RowKey = role.Id;
			await _cloudTableAccess.InsertOrMerge(role);

			return new MapRIdentityResult();
		}

		public Task<IdentityResult> DeleteAsync(Data.Models.MapRRole role, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
		}

		public async Task<Data.Models.MapRRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
			var result = await _cloudTableAccess.Retrieve(_partition, roleId);
			
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
			await _cloudTableAccess.Replace(role);

			return new MapRIdentityResult();
		}
	}
}
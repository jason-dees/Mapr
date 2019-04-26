﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MapR.DataStores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores {
	public partial class UserStore : IUserStore<Data.Models.MapRUser> {
		const string _partition = "MapR";
		readonly CloudTable _userTable;
		readonly IMapper _mapper;
		public UserStore(CloudTableClient tableClient, IMapper mapper) {
			_userTable = tableClient.GetTableReference("users");
			_mapper = mapper;
		}

		public async Task<IdentityResult> CreateAsync(Data.Models.MapRUser userModel, CancellationToken cancellationToken) {
			var user = _mapper.Map<MapRUser>(userModel);
			user.PartitionKey = _partition;
            user.RowKey = user.NameIdentifier;
			TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(user);

			await _userTable.ExecuteAsync(insertOrMergeOperation);

			return new MapRIdentityResult();
		}

		public Task<IdentityResult> DeleteAsync(Data.Models.MapRUser user, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
		}

		public async Task<Data.Models.MapRUser> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            var idQuery = TableQuery.GenerateFilterCondition("NameIdentifier", QueryComparisons.Equal, userId);
            var query = new TableQuery<MapRUser>()
                .Where(idQuery);
            return (await _userTable.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
        }

		public async Task<Data.Models.MapRUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            var providerKeyQuery = TableQuery.GenerateFilterCondition("UserName", QueryComparisons.Equal, normalizedUserName);
            var query = new TableQuery<MapRUser>()
                .Where(providerKeyQuery); 
            return (await _userTable.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
        }

		public async Task<string> GetNormalizedUserNameAsync(Data.Models.MapRUser user, CancellationToken cancellationToken) {
			return user.Email;
		}

		public async Task<string> GetUserIdAsync(Data.Models.MapRUser user, CancellationToken cancellationToken) {
			return user.Email;
		}

		public async Task<string> GetUserNameAsync(Data.Models.MapRUser user, CancellationToken cancellationToken) {
			return user.UserName;
		}

		public async Task SetNormalizedUserNameAsync(Data.Models.MapRUser user, string normalizedName, CancellationToken cancellationToken) {
			
		}

		public async Task SetUserNameAsync(Data.Models.MapRUser user, string userName, CancellationToken cancellationToken) {
			
		}

		public async Task<IdentityResult> UpdateAsync(Data.Models.MapRUser userModel, CancellationToken cancellationToken) {
			var user = _mapper.Map<MapRUser>(userModel);
			TableOperation operation = TableOperation.Replace(user);

			await _userTable.ExecuteAsync(operation);

			return new MapRIdentityResult();
		}
	}
}

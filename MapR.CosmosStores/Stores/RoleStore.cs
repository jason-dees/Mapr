using MapR.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MapR.DataStores.Stores {
    public class RoleStore : IRoleStore<Data.Models.MapRRole> {
        const string _role = "User";
        public Task<IdentityResult> CreateAsync(MapRRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(MapRRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public void Dispose() {
        }

        public async Task<MapRRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
            return new MapRRole {
                Id = _role,
                Name = _role,
                NormalizedName = _role
            };
        }

        public async Task<MapRRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
            return new MapRRole {
                Id = _role,
                Name = _role,
                NormalizedName = _role
            };
        }

        public async Task<string> GetNormalizedRoleNameAsync(MapRRole role, CancellationToken cancellationToken) {
            return _role;
        }

        public async Task<string> GetRoleIdAsync(MapRRole role, CancellationToken cancellationToken) {
            return _role;
        }

        public async Task<string> GetRoleNameAsync(MapRRole role, CancellationToken cancellationToken) {
            return _role;
        }

        public async Task SetNormalizedRoleNameAsync(MapRRole role, string normalizedName, CancellationToken cancellationToken) {
            
        }

        public Task SetRoleNameAsync(MapRRole role, string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(MapRRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}

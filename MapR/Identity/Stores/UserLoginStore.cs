using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MapR.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace MapR.Identity.Stores {
    public partial class UserStore : IUserLoginStore<ApplicationUser> {

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}

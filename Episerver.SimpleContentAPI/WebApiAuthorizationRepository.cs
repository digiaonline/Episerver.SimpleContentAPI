using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Episerver.SimpleContentAPI
{
    public class WebApiAuthorizationRepository : IDisposable
    {

        private WebApiAuthorizationDbContext _context;
        private UserManager<IdentityUser> _usrMgr;

        public WebApiAuthorizationRepository()
        {
            _context = new WebApiAuthorizationDbContext();
            _usrMgr = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_context));
        }

        public async Task<IdentityUser> GetUserAsync(string username, string password)
        {
            IdentityUser usr = await _usrMgr.FindAsync(username, password);
            return usr;
        }

        public async Task<IList<string>> GetUserRoles(string userId)
        {
            IList<string> roles = await _usrMgr.GetRolesAsync(userId);
            return roles;
        }

        public void Dispose()
        {
            _context.Dispose();
            _usrMgr.Dispose();
        }

    }

}
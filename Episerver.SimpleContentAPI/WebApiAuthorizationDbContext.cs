using Microsoft.AspNet.Identity.EntityFramework;

namespace Episerver.SimpleContentAPI
{
    internal class WebApiAuthorizationDbContext : IdentityDbContext
    {
        // Base value is the name of the database at connection strings.
        public WebApiAuthorizationDbContext() : base("EPiServerDB")
        {
        }
    }
}
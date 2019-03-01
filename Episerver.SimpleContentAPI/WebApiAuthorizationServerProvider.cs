using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Episerver.SimpleContentAPI
{
    public class WebApiAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            using (WebApiAuthorizationRepository authRepository = new WebApiAuthorizationRepository())
            {
                IdentityUser user = await authRepository.GetUserAsync(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }

                // Set list of claims
                List<Claim> claims = new List<Claim>
                {
                    // Add user name to claims
                    new Claim(ClaimTypes.Name, context.UserName)
                };

                // Set roles to claims                
                foreach (var role in await authRepository.GetUserRoles(user.Id))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Create oAuthIdentity with claims
                ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims, context.Options.AuthenticationType);

                // Validate Identity
                context.Validated(new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties() { }));

            }

        }

    }
}
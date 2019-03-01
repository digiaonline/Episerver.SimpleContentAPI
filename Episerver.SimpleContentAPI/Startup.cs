using System;
using System.Web;
using System.Web.Http;
using EPiServer.Cms.UI.AspNetIdentity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(Episerver.SimpleContentAPI.Startup))]

namespace Episerver.SimpleContentAPI
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {

            // Add CMS integration for ASP.NET Identity
            app.AddCmsAspNetIdentity<ApplicationUser>();

            // Remove to block registration of administrators
            app.UseAdministratorRegistrationPage(() => HttpContext.Current.Request.IsLocal);

            // Use cookie authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(Global.LoginPath),
                Provider = new CookieAuthenticationProvider
                {
                    // If the "/util/login.aspx" has been used for login otherwise you don't need it you can remove OnApplyRedirect.
                    OnApplyRedirect = cookieApplyRedirectContext =>
                    {
                        app.CmsOnCookieApplyRedirect(cookieApplyRedirectContext, cookieApplyRedirectContext.OwinContext.Get<ApplicationSignInManager<ApplicationUser>>());
                    },

                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager<ApplicationUser>, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => manager.GenerateUserIdentityAsync(user))
                }
            });

            // Configure the use of Bearer Tokens
            // Do not set the token expiry too high, as when editing user roles we don't have revoke mechanism
            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(6),
                Provider = new WebApiAuthorizationServerProvider()
            });

            // Configure Web API
            HttpConfiguration config = new HttpConfiguration();
            // Enable CORS
            config.EnableCors();
            // Map Attribute Routes
            config.MapHttpAttributeRoutes();
            // Remove XmlFormatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            // Ignore reference loops
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            // Apply Web API configuration for self-host. 
            app.UseWebApi(config);

        }
    }
}

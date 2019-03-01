using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Episerver.SimpleContentAPI.Business
{
    // Custom Web Api authorize attribute to replace Episerver login screen when HttpStatusCode is Forbidden 
    public class WebApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden
            };
        }
    }
}
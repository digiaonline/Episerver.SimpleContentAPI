using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web.Http;
using Episerver.SimpleContentAPI.Business;
using System.Web.Http.Cors;
using JOS.ContentSerializer;
using System.Linq;

namespace Episerver.SimpleContentAPI.Controllers
{
    [RoutePrefix("api")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContentTypeAPIController : ApiController
    {

        private ContentLocator contentLocator;
        private IContentRepository contentRepository;
        private readonly IContentTypeRepository pageTypeRepository;

        public ContentTypeAPIController()
        {
            contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            contentLocator = ServiceLocator.Current.GetInstance<ContentLocator>();
            pageTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
        }

        [Route("articles")]
        public IHttpActionResult GetArticles()
        {
            return GetAllOfType("ArticlePage");
        }

        [Route("articles/{pagereference}")]
        public IHttpActionResult GetArticle(PageReference pagereference)
        {
            return GetSinglePage(pagereference);
        }

        private IHttpActionResult GetSinglePage(PageReference pageReference = null)
        {
            if (pageReference == null)
            {
                return Content(HttpStatusCode.BadRequest, "Missing page reference.");
            }
            try
            {
                var catalog = contentRepository.Get<IContent>(pageReference);
                return Json(JObject.Parse(catalog.ToJson()));
            }
            catch
            {
                return Content(HttpStatusCode.NotFound, "Page not found");
            }
        }

        private IHttpActionResult GetAllOfType(string type, bool recursive = true)
        {

            if (string.IsNullOrEmpty(type) || pageTypeRepository.Load(type) == null)
            {
                return Content(HttpStatusCode.BadRequest, "Requires valid content type");
            }

            var pageReference = SiteDefinition.Current.StartPage.ToPageReference();
            var pageTypeId = pageTypeRepository.Load(type).ID;
            var results = contentLocator.FindPagesByPageType(pageReference, recursive, pageTypeId).ToList();
            var jsonResult = new JArray();

            foreach (var item in results)
            {
                jsonResult.Add(JObject.Parse(item.ToJson()));
            }

            return Json(jsonResult);

        }

    }
}
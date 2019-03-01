using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using JOS.ContentSerializer.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Episerver.SimpleContentAPI.Models.Pages
{
    /// <summary>
    /// Used primarily for publishing news articles on the website
    /// </summary>
    [SiteContentType(
        GroupName = Global.GroupNames.News,
        GUID = "AEECADF2-3E89-4117-ADEB-F8D43565D2F4")]
    [SiteImageUrl(Global.StaticGraphicsFolderPath + "page-type-thumbnail-article.png")]
    public class ArticlePage : StandardPage
    {
        // Include to Json response
        [ContentSerializerInclude]
        // Hide from the edit view
        [ScaffoldColumn(false)]
        public virtual string PageReference
        {
            get
            {
                return ContentLink.ToString();
            }
        }

        // Include to Json response
        [ContentSerializerInclude]
        // Hide from the edit view
        [ScaffoldColumn(false)]
        public virtual string ExternalUrl
        {
            get
            {
                if (ServiceLocator.Current != null && HttpContext.Current != null)
                {
                    return ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(ContentLink, Language.Name);
                }
                return string.Empty;
            }
        }
    }
}

using Castle.Core.Internal;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.Web;
using EPiServer.Web.Routing;
using JOS.ContentSerializer;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Episerver.SimpleContentAPI.Business.Rendering
{
    // See https://blog.mathiaskunto.com/2018/01/23/react-and-episerver-getting-friendly-urls-in-episervers-xhtml-fields-using-jos-contentserializer-for-react-support/
    public class CustomXhtmlStringPropertyHandler : IPropertyHandler<XhtmlString>
    {
        private readonly IContentLoader _contentLoader;
        private readonly HttpContextBase _httpContextBase;
        private readonly UrlResolver _urlResolver;

        public CustomXhtmlStringPropertyHandler(IContentLoader contentLoader, HttpContextBase httpContextBase, UrlResolver urlResolver)
        {
            _contentLoader = contentLoader;
            _httpContextBase = httpContextBase;
            _urlResolver = urlResolver;
        }

        public object Handle(XhtmlString value, PropertyInfo property, IContentData contentData)
        {
            HandleFragmentsFor(ref value);
            return value?.ToHtmlString();
        }

        private void HandleFragmentsFor(ref XhtmlString xhtmlString)
        {
            if (xhtmlString?.Fragments == null || !xhtmlString.Fragments.Any())
            {
                return;
            }

            xhtmlString = xhtmlString.CreateWritableClone();
            for (var i = 0; i < xhtmlString.Fragments.Count; i++)
            {
                var fragment = xhtmlString.Fragments[i];
                if (fragment is UrlFragment urlFragment && TryConvertUrlFragment(urlFragment, out var uFragment))
                {
                    xhtmlString.Fragments[i] = uFragment;
                    continue;
                }
            }

        }

        private bool TryConvertUrlFragment(UrlFragment fragment, out UrlFragment outFragment)
        {
            if ((fragment?.ReferencedPermanentLinkIds).IsNullOrEmpty())
            {
                // Not an internal EPiServer URL.
                outFragment = null;
                return false;
            }

            // We need to keep EPiServer's Context Mode while converting URLs (View mode, Edit mode, Preview mode).
            var mode = _httpContextBase?.Request?.RequestContext?.GetContextMode() ?? ContextMode.Default;
            var internalUrl = new UrlBuilder(fragment.InternalFormat);
            var friendlyUrl = _urlResolver.GetUrl(internalUrl, mode);
            outFragment = new UrlFragment(friendlyUrl);

            return true;
        }

    }
}

using EPiServer.Core;

namespace Episerver.SimpleContentAPI.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}

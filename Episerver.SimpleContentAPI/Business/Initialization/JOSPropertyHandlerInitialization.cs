using Episerver.SimpleContentAPI.Business.Rendering;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using JOS.ContentSerializer;
using JOS.ContentSerializer.Internal.Default;

namespace Episerver.SimpleContentAPI.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    [ModuleDependency(typeof(JOS.ContentSerializer.Internal.ContentSerializerInitalizationModule))]
    public class JOSPropertyHandlerInitialization : IConfigurableModule
    {
        public void Initialize(InitializationEngine context) { }
        public void Uninitialize(InitializationEngine context) { }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            // Force remove JOS default handler already registered, we don't need it.
            context.Services.RemoveAll<XhtmlStringPropertyHandler>();
            // Add our custon which hanles internal urls in xhtmlstring
            context.Services.AddSingleton<IPropertyHandler<XhtmlString>, CustomXhtmlStringPropertyHandler>();
        }
    }

}
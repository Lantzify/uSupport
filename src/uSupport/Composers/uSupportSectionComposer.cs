using Umbraco.Cms.Core.Composing;
using uSupport.Backoffice.Sections;
using Umbraco.Cms.Core.DependencyInjection;

namespace uSupport.Composers
{
	public class uSupportSectionComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.Sections().Append<uSupportSection>();
		}
	}
}
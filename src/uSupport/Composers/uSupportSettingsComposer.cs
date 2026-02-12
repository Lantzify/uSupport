using uSupport.Dtos.Settings;
using Umbraco.Cms.Core.Composing;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace uSupport.Composers
{
	public class uSupportSettingsComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{

			builder.Services.AddOptions<uSupportSettings>()
				.Bind(builder.Config.GetSection(uSupportSettings.uSupport));

			builder.Services.AddOptions<uSupportSettingsTicket>()
				.Bind(builder.Config.GetSection(uSupportSettingsTicket.Tickets));
		}
	}
}
using uSupport.Services;
using Umbraco.Cms.Core.Composing;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace uSupport.Composers
{
	public class uSupportServicesComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.Services.AddTransient<IuSupportTicketTypeService, uSupportTicketTypeService>()
							.AddTransient<IuSupportTicketStatusService, uSupportTicketStatusService>()
							.AddTransient<IuSupportTicketCommentService, uSupportTicketCommentService>()
							.AddTransient<IuSupportTicketService, uSupportTicketService>()
							.AddScoped<IuSupportSettingsService, uSupportSettingsService>();
		}
	}
}
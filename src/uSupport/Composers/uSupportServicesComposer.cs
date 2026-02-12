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
			builder.Services.AddScoped<IuSupportTicketTypeService, uSupportTicketTypeService>()
							.AddScoped<IuSupportTicketStatusService, uSupportTicketStatusService>()
							.AddScoped<IuSupportTicketCommentService, uSupportTicketCommentService>()
							.AddScoped<IuSupportTicketService, uSupportTicketService>()
							.AddScoped<IuSupportSettingsService, uSupportSettingsService>();
		}
	}
}
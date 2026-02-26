using uSupport.Notifications;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace uSupport.Composers
{
	public class uSupportNotificationComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.AddNotificationHandler<TicketHistoryNotification, TicketHistoryNotificationHandler>();
		}
	}
}
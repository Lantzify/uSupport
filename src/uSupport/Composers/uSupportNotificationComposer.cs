using uSupport.Notifications;
using Umbraco.Cms.Core.Composing;
using uSupport.Notifications.Tickets;
using uSupport.Notifications.Handlers;
using Umbraco.Cms.Core.DependencyInjection;

namespace uSupport.Composers
{
	public class uSupportNotificationComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.AddNotificationHandler<TicketHistoryNotification, TicketHistoryNotificationHandler>()
				.AddNotificationHandler<CreateTicketNotification, TicketHistoryNotificationHandler>()
				.AddNotificationHandler<EmailSendingNotification, TicketHistoryNotificationHandler>()
				.AddNotificationHandler<AddTicketCommentNotification, TicketHistoryNotificationHandler>();
		}
	}
}
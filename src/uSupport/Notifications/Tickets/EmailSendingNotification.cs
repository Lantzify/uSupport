using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications.Tickets
{
	public class EmailSendingNotification : INotification
	{
		public Guid TicketId { get; }
		public string ToAddress { get; }
		public string Subject { get; }
		public string TemplatePath { get; }

		public EmailSendingNotification(Guid ticketId, string toAddress, string subject, string templatePath)
		{
			TicketId = ticketId;
			ToAddress = toAddress;
			Subject = subject;
			TemplatePath = templatePath;
		}
	}
}
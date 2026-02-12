using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class CreateTicketStatusNotification : INotification
    {
        public uSupportTicketStatus TicketStatus { get; }
        public CreateTicketStatusNotification(uSupportTicketStatus ticketStatus)
        {
            TicketStatus = ticketStatus;
        }
    }
}
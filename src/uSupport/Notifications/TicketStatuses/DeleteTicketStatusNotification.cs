using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class DeleteTicketStatusNotification : INotification
    {
        public uSupportTicketStatus TicketStatus { get; }
        public DeleteTicketStatusNotification(uSupportTicketStatus ticketStatus)
        {
            TicketStatus = ticketStatus;
        }
    }
}
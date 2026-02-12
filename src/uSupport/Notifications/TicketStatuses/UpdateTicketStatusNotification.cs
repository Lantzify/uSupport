using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class UpdateTicketStatusNotification : INotification
    {
        public uSupportTicketStatus TicketStatus { get; }
        public UpdateTicketStatusNotification(uSupportTicketStatus ticketStatus)
        {
            TicketStatus = ticketStatus;
        }
    }
}
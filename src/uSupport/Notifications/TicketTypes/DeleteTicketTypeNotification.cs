using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class DeleteTicketTypeNotification : INotification
    {
        public uSupportTicketType TicketType { get; }
        public DeleteTicketTypeNotification(uSupportTicketType ticketType)
        {
            TicketType = ticketType;
        }
    }
}
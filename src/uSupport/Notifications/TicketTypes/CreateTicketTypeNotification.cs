using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class CreateTicketTypeNotification : INotification
    {
        public uSupportTicketType TicketType { get; }
        public CreateTicketTypeNotification(uSupportTicketType ticketType)
        {
            TicketType = ticketType;
        }
    }
}
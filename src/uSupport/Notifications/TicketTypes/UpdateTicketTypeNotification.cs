using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class UpdateTicketTypeNotification : INotification
    {
        public uSupportTicketType TicketType { get; }
        public UpdateTicketTypeNotification(uSupportTicketType ticketType)
        {
            TicketType = ticketType;
        }
    }
}
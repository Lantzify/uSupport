using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class CreateTicketNotification : INotification
    {
        public uSupportTicket Ticket { get; }
        public CreateTicketNotification(uSupportTicket ticket)
        {
            Ticket = ticket;
        }
    }
}
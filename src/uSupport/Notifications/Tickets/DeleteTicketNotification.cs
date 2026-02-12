using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class DeleteTicketNotification : INotification
    {
        public uSupportTicket Ticket { get; }
        public DeleteTicketNotification(uSupportTicket ticket)
        {
            Ticket = ticket;
        }
    }
}
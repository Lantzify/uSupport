using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class UpdateTicketNotification : INotification
    {
        public uSupportTicket Ticket { get; }
        public UpdateTicketNotification(uSupportTicket ticket)
        {
            Ticket = ticket;
        }
    }
}
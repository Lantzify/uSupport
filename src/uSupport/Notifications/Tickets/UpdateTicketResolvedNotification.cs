using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class UpdateTicketResolvedNotification : INotification
    {
        public uSupportTicket Ticket { get; }
        public UpdateTicketResolvedNotification(uSupportTicket ticket)
        {
            Ticket = ticket;
        }
    }
}
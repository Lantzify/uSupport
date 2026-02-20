using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class UpdateTicketSendEmailNotification : INotification
    {
        public uSupportTicket Ticket { get; }
        public UpdateTicketSendEmailNotification(uSupportTicket ticket)
        {
            Ticket = ticket;
        }
    }
}
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class AddTicketCommentNotification : INotification
    {
        public uSupportTicket Ticket { get; }
        public uSupportTicketComment Comment { get; }
        public AddTicketCommentNotification(uSupportTicket ticket, uSupportTicketComment comment)
        {
            Ticket = ticket;
            Comment = comment;
        }
    }
}
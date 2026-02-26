using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class TicketHistoryNotification : INotification
    {
		public int UserId { get; set; }
		public uSupportTicket OldTicket { get; }
        public uSupportTicket NewTicket { get; }
        public TicketHistoryNotification(int userId, uSupportTicket oldTicket, uSupportTicket newTicket)
        {
            UserId = userId;
            OldTicket = oldTicket;
            NewTicket = newTicket;
        }
    }
}
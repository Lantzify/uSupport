﻿#if NETCOREAPP
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class AddTicketCommentNotification : INotification
    {
        public uSupportTicket Ticket { get; }
        public AddTicketCommentNotification(uSupportTicket ticket)
        {
            Ticket = ticket;
        }
    }
}
#endif
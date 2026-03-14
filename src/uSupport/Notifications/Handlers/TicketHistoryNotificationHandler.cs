using uSupport.Dtos;
using System.Text.Json;
using Umbraco.Cms.Core.Events;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using uSupport.Notifications.Tickets;

namespace uSupport.Notifications.Handlers
{
	public class TicketHistoryNotificationHandler : INotificationHandler<TicketHistoryNotification>, 
		INotificationHandler<CreateTicketNotification>,
		INotificationHandler<EmailSendingNotification>,
		INotificationHandler<AddTicketCommentNotification>
	{
		private readonly IuSupportTicketHistoryService _uSupportTicketHistoryService;
		public TicketHistoryNotificationHandler(IuSupportTicketHistoryService uSupportTicketHistoryService)
		{
			_uSupportTicketHistoryService = uSupportTicketHistoryService;
		}

		public void Handle(TicketHistoryNotification notification)
		{
			var history = new uSupportTicketHistorySchema()
			{
				TicketId = notification.NewTicket.Id,
				UserId = notification.UserId,
				ActionType = "Updated",
			};


			var changes = new List<uSupportChange>();

			if (notification.OldTicket.TypeId != notification.NewTicket.TypeId)
			{
				changes.Add(new uSupportChange()
				{
					Field = "Type",
					Old = notification.OldTicket.TypeId.ToString(),
					New = notification.NewTicket.TypeId.ToString()
				});
			}

			if (notification.OldTicket.StatusId != notification.NewTicket.StatusId)
			{
				changes.Add(new uSupportChange()
				{
					Field = "Status",
					Old = notification.OldTicket.StatusId.ToString(),
					New = notification.NewTicket.StatusId.ToString()
				});
			}

			if (notification.OldTicket.InternalComment != notification.NewTicket.InternalComment)
			{
				changes.Add(new uSupportChange()
				{
					Field = "InternalComment",
					Old = notification.OldTicket?.InternalComment ?? "",
					New = notification.NewTicket?.InternalComment ?? ""
				});
			}

			if (!changes.Any())
				return;

			history.ChangesJson = JsonSerializer.Serialize(changes);

			if(!notification.NewTicket.Status.Active)
				history.ActionType = "Resolved";

			_uSupportTicketHistoryService.Create(history);
		}

		public void Handle(CreateTicketNotification notification)
		{
			_uSupportTicketHistoryService.Create(new uSupportTicketHistorySchema()
			{
				TicketId = notification.Ticket.Id,
				UserId = notification.Ticket.AuthorId,
				ActionType = "Created",
			});
		}

		public void Handle(EmailSendingNotification notification)
		{
			_uSupportTicketHistoryService.Create(new uSupportTicketHistorySchema()
			{
				TicketId = notification.TicketId,
				UserId = 0,
				ActionType = "SentEmail",
				ChangesJson = JsonSerializer.Serialize(new List<uSupportChange>()
				{
					new uSupportChange()
					{
						Field = "Email",
						New = notification.ToAddress
					}
				})
			});
		}

		public void Handle(AddTicketCommentNotification notification)
		{
			_uSupportTicketHistoryService.Create(new uSupportTicketHistorySchema()
			{
				TicketId = notification.Ticket.Id,
				UserId = notification.Comment.UserId,
				ActionType = "Comment",
				ChangesJson = JsonSerializer.Serialize(new List<uSupportChange>()
				{
					new uSupportChange()
					{
						Field = "Comment"
					}
				})
			});
		}
	}
}

using uSupport.Dtos;
using System.Text.Json;
using Umbraco.Cms.Core.Events;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;

namespace uSupport.Notifications
{
	public class TicketHistoryNotificationHandler : INotificationHandler<TicketHistoryNotification>
	{
		private readonly IuSupportTicketHistoryService _uSupportTicketHistoryService;
		public TicketHistoryNotificationHandler(IuSupportTicketHistoryService uSupportTicketHistoryService)
		{
			_uSupportTicketHistoryService = uSupportTicketHistoryService;
		}

		//Update
		public void Handle(TicketHistoryNotification notification)
		{
			var history = new uSupportTicketHistorySchema()
			{
				TicketId = notification.NewTicket.Id,
				UserId = notification.UserId,
				ActionType = notification.OldTicket == null ? "Created" : "Updated",
			};

			if (notification.OldTicket == null)
			{
				_uSupportTicketHistoryService.Create(history);
				return;
			}

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


			if (notification.OldTicket.PropertyValue != notification.NewTicket.PropertyValue)
			{
				changes.Add(new uSupportChange()
				{
					Field = "Property value",
					Old = notification.OldTicket.PropertyValue,
					New = notification.NewTicket.PropertyValue
				});
			}

			if (!changes.Any())
				return;

			history.ChangesJson = JsonSerializer.Serialize(changes);

			_uSupportTicketHistoryService.Create(history);
		}
	}
}

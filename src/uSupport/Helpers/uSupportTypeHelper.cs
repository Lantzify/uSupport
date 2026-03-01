using uSupport.Dtos;
using uSupport.Dtos.Tables;
using System.Collections.Generic;
using uSupport.Migrations.Schemas;

namespace uSupport.Helpers
{
	public static class uSupportTypeHelper
	{
		public static uSupportTicketSchema ConvertDtoToSchema(this uSupportTicket ticket)
		{
			return new uSupportTicketSchema() { 
				AuthorId = ticket.AuthorId,
				ExternalTicketId = ticket.ExternalTicketId,
				LastUpdatedBy = ticket.LastUpdatedBy,
				LastUpdated = ticket.LastUpdated,
				Id = ticket.Id,
				Resolved = ticket.Resolved,
				StatusId = ticket.StatusId,
				Submitted = ticket.Submitted,
				Summary = ticket.Summary,
				Title = ticket.Title,
				InternalComment = ticket.InternalComment,
				PropertyValue = ticket.PropertyValue,
				TypeId = ticket.TypeId
			};
		}

		public static uSupportTicketTypeSchema ConvertDtoToSchema(this uSupportTicketType ticketType)
		{
			return new uSupportTicketTypeSchema()
			{
				Id = ticketType.Id,
				Alias = ticketType.Alias,
				Color = ticketType.Color,
				Description = ticketType.Description,
				Icon = ticketType.Icon,
				Name = ticketType.Name,
				Order = ticketType.Order,
				PropertyDescription = ticketType.PropertyDescription,
				PropertyId = ticketType.PropertyId,
				PropertyName = ticketType.PropertyName,
				PropertyView = ticketType.PropertyView
			};
		}

		public static uSupportTicketStatusSchema ConvertDtoToSchema(this uSupportTicketStatus ticketStatus)
		{
			return new uSupportTicketStatusSchema()
			{
				Id = ticketStatus.Id,
				Alias = ticketStatus.Alias,
				Color = ticketStatus.Color,
				Icon = ticketStatus.Icon,
				Name = ticketStatus.Name,
				Order = ticketStatus.Order,
				Active = ticketStatus.Active,
				Default = ticketStatus.Default
			};
		}

		public static uSupportTicketComment ConvertSchemaToDto(this uSupportTicketCommentSchema comment)
		{
			return new uSupportTicketComment()
			{
				Comment = comment.Comment,
				Date = comment.Date,
				Id = comment.Id,
				TicketId = comment.TicketId,
				UserId = comment.UserId
			};
		}

		public static uSupportTicketCommentSchema ConvertDtoToSchema(this uSupportTicketComment comment)
		{
			return new uSupportTicketCommentSchema()
			{
				Comment = comment.Comment,
				Date = comment.Date,
				Id = comment.Id,
				TicketId = comment.TicketId,
				UserId = comment.UserId
			};
		}

		public static PreviewDeleteItemDto ConvertToDeletePreviewDto(this uSupportTicket ticket)
		{
			return new PreviewDeleteItemDto()
			{
				Id = ticket.Id,
				Name = ticket.Title,
				Type = "ticket",
				Path = ""
			};
		}

		public static IEnumerable<PreviewDeleteItemDto> ConvertToDeletePreviewDto(this IEnumerable<uSupportTicket> ticket)
		{
			var tickets = new List<PreviewDeleteItemDto>();
			foreach (var item in ticket)
				tickets.Add(item.ConvertToDeletePreviewDto());
			
			return tickets;
		}
	}
}
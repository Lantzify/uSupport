using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Infrastructure.Migrations;

namespace uSupport.Migrations.Updates._2._3._0
{
	public class RemoveOrphanTicketComments : uSupportMigrationBase
	{
		private readonly IuSupportTicketService _uSupportTicketService;
		private readonly IuSupportTicketCommentService _uSupportTicketCommentService;


		public RemoveOrphanTicketComments(
			IMigrationContext context,
			IuSupportTicketService uSupportTicketService,
			IuSupportTicketCommentService uSupportTicketCommentService) : base(context)
		{
			_uSupportTicketService = uSupportTicketService;
			_uSupportTicketCommentService = uSupportTicketCommentService;
		}

		protected override void DoMigrate()
		{
			var allComments = _uSupportTicketCommentService.GetAll();
			List<Guid> ticketIds = new List<Guid>();
			List<Guid> deletedTicketIds = new List<Guid>();

			foreach (var comment in allComments)
			{
				if (ticketIds.Contains(comment.TicketId))
					continue; 

				if (deletedTicketIds.Contains(comment.TicketId))
				{
					_uSupportTicketCommentService.Delete(comment.Id);
					continue;
				}

				var ticket = _uSupportTicketService.Get(comment.TicketId);
				if (ticket == null)
				{
					deletedTicketIds.Add(comment.TicketId);
					_uSupportTicketCommentService.Delete(comment.Id);
				}
				else
				{
					ticketIds.Add(comment.TicketId);
				}
			}
		}
	}
}

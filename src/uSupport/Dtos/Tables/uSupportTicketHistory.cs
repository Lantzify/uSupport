using NPoco;
using Umbraco.Cms.Core.Models.ContentEditing;

namespace uSupport.Dtos.Tables
{
	public class uSupportTicketHistory
	{
		public Guid Id { get; set; }
		public Guid TicketId { get; set; }
		public string ActionType { get; set; }

		[ResultColumn]
		public IEnumerable<uSupportChange>? Changes { get; set; }
		public string? ChangesJson { get; set; }
		public int UserId { get; set; }

		[ResultColumn]
		public UserDisplay? User { get; set; }
		public DateTime Date { get; set; }
	}
}
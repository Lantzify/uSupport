using Umbraco.Cms.Core.Models.ContentEditing;

namespace uSupport.Dtos.Tables
{
	public class uSupportTicketComment
	{
		public Guid Id { get; set; }
		public Guid TicketId { get; set; }
		public int UserId { get; set; }
		public UserDisplay User { get; set; }
		public DateTime Date { get; set; }
		public string Comment { get; set; }
	}
}
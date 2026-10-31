using NPoco;
using uSupport.Dtos;

namespace uSupport.Dtos.Tables
{
	public class uSupportTicketComment
	{
		public Guid Id { get; set; }
		public Guid TicketId { get; set; }
		public int UserId { get; set; }

		[ResultColumn]
		public uSupportUser? User { get; set; }
		public DateTime Date { get; set; }
		public string Comment { get; set; }
	}
}
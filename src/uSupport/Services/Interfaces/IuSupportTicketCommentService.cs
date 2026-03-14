using uSupport.Dtos;
using uSupport.Dtos.Tables;
using uSupport.Migrations.Schemas;

namespace uSupport.Services.Interfaces
{
	public interface IuSupportTicketCommentService
	{
		IEnumerable<uSupportTicketComment> GetCommentsFromTicketId(Guid ticketId);
		uSupportPage<uSupportTicketComment> GetPagedCommentsForTicket(Guid ticketId, long page);
		IEnumerable<uSupportTicketComment> GetAll();
		uSupportTicketComment Get(Guid id);
		uSupportTicketComment Create(uSupportTicketCommentSchema comment);
		uSupportTicketComment Update(uSupportTicketCommentSchema comment);
		void DeleteByTicketId(Guid ticketId);
		void Delete(Guid id);
	}
}
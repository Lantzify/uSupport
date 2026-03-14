using uSupport.Dtos;
using uSupport.Dtos.Tables;
using uSupport.Migrations.Schemas;

namespace uSupport.Services.Interfaces
{
	public interface IuSupportTicketHistoryService
	{
		IEnumerable<uSupportTicketHistory> GetByTicketId(Guid ticketId);
		uSupportPage<uSupportTicketHistory> GetPagedByTicketId(Guid ticketId, long page);
		uSupportTicketHistory Get(Guid id);
		uSupportTicketHistory Create(uSupportTicketHistorySchema history);
		uSupportTicketHistory Update(uSupportTicketHistorySchema history);
		void DeleteByTicketId(Guid ticketId);
		void Delete(Guid id);
	}
}
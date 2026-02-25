using uSupport.Dtos;
using uSupport.Dtos.Tables;
using uSupport.Migrations.Schemas;

namespace uSupport.Services.Interfaces
{
	public interface IuSupportTicketService
	{
		IEnumerable<uSupportTicket> GetAll();
		uSupportPage<uSupportTicket> GetPagedResolvedTickets(long page, string? searchTerm = null);
		uSupportPage<uSupportTicket> GetPagedActiveTickets(long page, string? searchTerm = null);
		bool AnyResolvedTickets();
		uSupportTicket Get(Guid id);
		uSupportTicket Create(uSupportTicketSchema ticket);
		uSupportTicket Update(uSupportTicketSchema ticketDto);
		void Delete(Guid id);
		void ClearTicketCache();
	}
}
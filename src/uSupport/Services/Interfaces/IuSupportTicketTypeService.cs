using System;
using uSupport.Dtos.Tables;
using System.Collections.Generic;
using uSupport.Migrations.Schemas;

namespace uSupport.Services.Interfaces
{
	public interface IuSupportTicketTypeService
	{
		IEnumerable<uSupportTicketType> GetAll();
		uSupportTicketType Get(Guid id);
		IEnumerable<uSupportTicketType> GetByIds(List<Guid> ids);
		Guid? GetTypeIdFromName(string name);
		uSupportTicketType Create(uSupportTicketTypeSchema ticketType);
		uSupportTicketType Update(uSupportTicketTypeSchema ticketType);
		void Delete(Guid id);
		int GetTypesCount();
	}
}
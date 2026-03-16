using NPoco;
using uSupport.Dtos.Tables;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Infrastructure.Scoping;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketTypeService : uSupportServiceBase<uSupportTicketType, uSupportTicketTypeSchema>, IuSupportTicketTypeService
	{
		public uSupportTicketTypeService(IScopeProvider scopeProvider,
			IScopeAccessor scopeAccessor) : base(TicketTypeTableAlias, scopeProvider, scopeAccessor)
		{ }

		public IEnumerable<uSupportTicketType> GetAll()
		{
			var context = GetScope();

			try
			{
				var sql = new Sql()
								.Select("*")
								.From(TicketTypeTableAlias)
								.OrderBy("[Order]");

				return context.Scope.Database.Fetch<uSupportTicketType>(sql);
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public Guid? GetTypeIdFromName(string name)
		{
			var context = GetScope();
			try
			{
				var ticketStatus = context.Scope.Database.Query<uSupportTicketType>($"SELECT Id FROM {TicketTypeTableAlias} WHERE [Name] = @name", new { name }).FirstOrDefault();
				return ticketStatus?.Id;
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public int GetTypesCount()
		{
			var context = GetScope();

			try
			{
				return context.Scope.Database.Query<uSupportTicketType>($"SELECT [Order] FROM {TicketTypeTableAlias}").ToList().Count;
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}
	}
}
using uSupport.Dtos.Tables;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Infrastructure.Scoping;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketStatusService : uSupportServiceBase<uSupportTicketStatus, uSupportTicketStatusSchema>, IuSupportTicketStatusService
	{

		public uSupportTicketStatusService(IScopeProvider scopeProvider,
			IScopeAccessor scopeAccessor) : base(TicketStatusTableAlias, scopeProvider, scopeAccessor)
		{ }

		public uSupportTicketStatus GetDefaultStatus()
		{
			var context = GetScope();
			try
			{
				return context.Scope.Database.Query<uSupportTicketStatus>($"SELECT * FROM {TicketStatusTableAlias} WHERE [Default] = '1'").FirstOrDefault();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public IEnumerable<uSupportTicketStatus> GetActiveStatuses()
		{
			var context = GetScope();
			try
			{
				var statuses = context.Scope.Database.Query<uSupportTicketStatus>($"SELECT * FROM {TicketStatusTableAlias} WHERE [Active] = '1'");
				
				if (context.Created)
					context.Scope.Complete();

				return statuses.ToList();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public IEnumerable<uSupportTicketStatus> GetResolvedStatuses()
		{
			var context = GetScope();
			try
			{
				var statuses = context.Scope.Database.Query<uSupportTicketStatus>($"SELECT * FROM {TicketStatusTableAlias} WHERE [Active] = '0'");
				return statuses.ToList();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public IEnumerable<uSupportTicketStatus> GetAll()
		{
			var context = GetScope();
			try
			{
				return context.Scope.Database.Query<uSupportTicketStatus>($"SELECT * FROM {TicketStatusTableAlias} ORDER BY [Order]");
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public Guid GetStatusIdFromName(string name)
		{
			var context = GetScope();
			try
			{
				var ticketStatus = context.Scope.Database.Query<uSupportTicketStatus>($"SELECT Id FROM {TicketStatusTableAlias} WHERE [Name] = @name", new { name }).FirstOrDefault();
				return ticketStatus.Id;
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public int GetStatusCount()
		{
			var context = GetScope();
			try
			{
				return context.Scope.Database.Query<int>($"SELECT COUNT([Order]) FROM {TicketStatusTableAlias}").FirstOrDefault();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}
	}
}
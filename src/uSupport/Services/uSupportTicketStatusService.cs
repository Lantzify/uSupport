using uSupport.Dtos.Tables;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Infrastructure.Scoping;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketStatusService : uSupportServiceBase<uSupportTicketStatus, uSupportTicketStatusSchema>, IuSupportTicketStatusService
	{
		private static IScopeProvider _scopeProvider;

		public uSupportTicketStatusService(IScopeProvider scopeProvider) : base(TicketStatusTableAlias, scopeProvider)
		{
			_scopeProvider = scopeProvider;
		}

		public uSupportTicketStatus GetDefaultStatus()
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var status = db.Query<uSupportTicketStatus>($"SELECT * FROM {TicketStatusTableAlias} WHERE [Default] = '1'").FirstOrDefault();

				return status;
			}
		}

		public IEnumerable<uSupportTicketStatus> GetActiveStatuses()
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var statuses = db.Query<uSupportTicketStatus>($"SELECT * FROM {TicketStatusTableAlias} WHERE [Active] = '1'");

				return statuses.ToList();
			}
		}

		public IEnumerable<uSupportTicketStatus> GetResolvedStatuses()
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var statuses = db.Query<uSupportTicketStatus>($"SELECT * FROM {TicketStatusTableAlias} WHERE [Active] = '0'");

				return statuses.ToList();
			}
		}

		public IEnumerable<uSupportTicketStatus> GetAll()
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var ticketStatus = db.Query<uSupportTicketStatus>($"SELECT * FROM {TicketStatusTableAlias} ORDER BY [Order]");

				return ticketStatus;
			}
		}

		public Guid GetStatusIdFromName(string name)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var ticketStatus = db.Query<uSupportTicketStatus>($"SELECT Id FROM {TicketStatusTableAlias} WHERE [Name] = @name", new { name }).FirstOrDefault();

				return ticketStatus.Id;
			}
		}

		public int GetStatusCount()
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				return db.Query<int>($"SELECT COUNT([Order]) FROM {TicketStatusTableAlias}").FirstOrDefault();
			}
		}
	}
}
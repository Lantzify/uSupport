using NPoco;
using uSupport.Dtos;
using System.Text.Json;
using uSupport.Dtos.Tables;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Infrastructure.Scoping;
using uSupport.Helpers;
using static uSupport.Helpers.uSupportPageHelper;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketHistoryService : uSupportServiceBase<uSupportTicketHistory, uSupportTicketHistorySchema>, IuSupportTicketHistoryService
	{
		private readonly uSupportUserHelper _userHelper;

		public uSupportTicketHistoryService(uSupportUserHelper userHelper,
			IScopeProvider scopeProvider,
			IScopeAccessor scopeAccessor) : base(TicketHistoryTableAlias, scopeProvider, scopeAccessor)
		{
			_userHelper = userHelper;
		}

		public IEnumerable<uSupportTicketHistory> GetByTicketId(Guid ticketId)
		{
			var context = GetScope();
			try
			{
				var sqlCount = new Sql()
					.Select($"*")
					.From(TicketHistoryTableAlias)
					.Where($"{TicketHistoryTableAlias}.TicketId = UPPER('{ticketId}')")
					.OrderBy("Date DESC");

				var history = context.Scope.Database.Fetch<uSupportTicketHistory>(sqlCount);

				foreach (var item in history)
				{
					item.User = _userHelper.GetUser(item.UserId);

					if(!string.IsNullOrWhiteSpace(item.ChangesJson))
						item.Changes = JsonSerializer.Deserialize<IEnumerable<uSupportChange>>(item.ChangesJson);
				}

				return history;
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}


		public uSupportPage<uSupportTicketHistory> GetPagedByTicketId(Guid ticketId, long page)
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
					.Select("*")
					.From(TicketHistoryTableAlias)
					.Where($"{TicketHistoryTableAlias}.TicketId = UPPER('{ticketId}')")
					.OrderBy($"{TicketHistoryTableAlias}.Date DESC");

				var sqlCount = new Sql()
					.Select($"COUNT({TicketHistoryTableAlias}.Id)")
					.From(TicketHistoryTableAlias)
					.Where($"{TicketHistoryTableAlias}.TicketId = UPPER('{ticketId}')");

				var historyCount = context.Scope.Database.Fetch<int>(sqlCount).FirstOrDefault();
				var history = context.Scope.Database.SkipTake<uSupportTicketHistory>((page - 1) * PageSize, PageSize, sql);

				foreach (var item in history)
				{
					item.User = _userHelper.GetUser(item.UserId);

					if (!string.IsNullOrWhiteSpace(item.ChangesJson))
						item.Changes = JsonSerializer.Deserialize<IEnumerable<uSupportChange>>(item.ChangesJson);
				}

				return MapPageToUSupportPage(history, historyCount, page, PageSize);
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public void DeleteByTicketId(Guid ticketId)
		{
			var context = GetScope();
			try
			{
				context.Scope.Database.Delete<uSupportTicketHistory>($"WHERE {TicketHistoryTableAlias}.TicketId = UPPER('{ticketId}')");

				if (context.Created)
					context.Scope.Complete();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}
	}
}
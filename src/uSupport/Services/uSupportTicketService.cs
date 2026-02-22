using NPoco;
using uSupport.Dtos;
using uSupport.Extensions;
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Cache;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Infrastructure.Scoping;
using static uSupport.Helpers.uSupportPageHelper;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketService : uSupportServiceBase<uSupportTicket, uSupportTicketSchema>, IuSupportTicketService
	{
		private readonly AppCaches _appCaches;
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;

		public uSupportTicketService(AppCaches appCaches,
			IScopeProvider scopeProvider,
			IScopeAccessor scopeAccessor,
			IuSupportTicketStatusService uSupportTicketStatusService) : base(TicketTableAlias, scopeProvider, scopeAccessor)
		{
			_appCaches = appCaches;
			_uSupportTicketStatusService = uSupportTicketStatusService;
		}

		public IEnumerable<uSupportTicket> GetAll()
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
								.Select("*")
								.From(TicketTableAlias)
								.OrderBy("Submitted");

				return context.Scope.Database.Fetch<uSupportTicket>(sql);
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public uSupportPage<uSupportTicket> GetPagedActiveTickets(long page)
		{
			var context = GetScope();
			try
			{
				var statuses = _uSupportTicketStatusService.GetActiveStatuses().ConvertStatusesToSql();

				var sql = new Sql()
					.Select("*")
					.From(TicketTableAlias)
					.GetFullTicket()
					.Where($"StatusId IN ({statuses})")
					.OrderBy("Submitted");

				var sqlCount = new Sql()
					.Select("COUNT(Id)")
					.From(TicketTableAlias)
					.Where($"StatusId IN ({statuses})");

				var ticketCount = context.Scope.Database.Fetch<int>(sqlCount).FirstOrDefault();
				var tickets = context.Scope.Database.SkipTake<uSupportTicket>((page - 1) * PageSize, PageSize, sql);

				return MapPageToUSupportPage(tickets, ticketCount, page, PageSize);
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}

		}

		public uSupportPage<uSupportTicket> GetPagedResolvedTickets(long page)
		{
			var context = GetScope();
			try
			{
				var statuses = _uSupportTicketStatusService.GetResolvedStatuses().ConvertStatusesToSql();
				var sql = new Sql()
					.Select("*")
					.From(TicketTableAlias)
					.GetFullTicket()
					.Where($"StatusId IN ({statuses})")
					.OrderBy("Submitted");

				var sqlCount = new Sql()
					.Select("Id")
					.From(TicketTableAlias)
					.Where($"StatusId IN ({statuses})");

				var ticketCount = context.Scope.Database.Fetch<uSupportTicket>(sqlCount).ToList().Count;
				var tickets = context.Scope.Database.SkipTake<uSupportTicket>((page - 1) * PageSize, PageSize, sql);

				return MapPageToUSupportPage(tickets, ticketCount, page, PageSize);
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public bool AnyResolvedTickets()
		{
			var context = GetScope();
			try
			{
				var statuses = _uSupportTicketStatusService.GetResolvedStatuses().ConvertStatusesToSql();
				var sqlCount = new Sql()
					.Select("Id")
					.From(TicketTableAlias)
					.Where($"StatusId IN ({statuses})");

				return context.Scope.Database.Fetch<uSupportTicket>(sqlCount).Any();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public override uSupportTicket Get(Guid id)
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
								.Select("*")
								.From(TicketTableAlias)
								.GetFullTicket()
								.Where($"{TicketTableAlias}.Id = UPPER('{id}')");

				return context.Scope.Database.Fetch<uSupportTicket>(sql).FirstOrDefault();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public override uSupportTicket Update(uSupportTicketSchema dto)
		{
			var context = GetScope();
			try
			{
				context.Scope.Database.Update(dto);

				if (context.Created)
					context.Scope.Complete();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}

			return Get(dto.Id);
		}

		public void ClearTicketCache()
		{
			_appCaches.RuntimeCache.ClearByRegex("uSupportPaged");
		}
	}
}
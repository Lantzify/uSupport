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
		private static IScopeProvider _scopeProvider;	
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;

		public uSupportTicketService(AppCaches appCaches,
        IScopeProvider scopeProvider,
			IuSupportTicketStatusService uSupportTicketStatusService) : base(TicketTableAlias, scopeProvider)
		{
			_appCaches = appCaches;
			_scopeProvider = scopeProvider;
			_uSupportTicketStatusService = uSupportTicketStatusService;
		}

		public IEnumerable<uSupportTicket> GetAll()
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var sql = new Sql()
					.Select("*")
					.From(TicketTableAlias)
					.OrderBy("Submitted");

				return scope.Database.Fetch<uSupportTicket>(sql);
			}
		}

		public uSupportPage<uSupportTicket> GetPagedActiveTickets(long page)
		{			
			using (var scope = _scopeProvider.CreateScope())
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

				var ticketCount = scope.Database.Fetch<int>(sqlCount).FirstOrDefault();
				var tickets = scope.Database.SkipTake<uSupportTicket>((page - 1) * PageSize, PageSize, sql);
					
				return MapPageToUSupportPage(tickets, ticketCount, page, PageSize);
			}
		}

		public uSupportPage<uSupportTicket> GetPagedResolvedTickets(long page)
		{
			using (var scope = _scopeProvider.CreateScope())
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

				var ticketCount = scope.Database.Fetch<uSupportTicket>(sqlCount).ToList().Count;
				var tickets = scope.Database.SkipTake<uSupportTicket>((page - 1) * PageSize, PageSize, sql);

				return MapPageToUSupportPage(tickets, ticketCount, page, PageSize);
			}
		}

		public bool AnyResolvedTickets()
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var statuses = _uSupportTicketStatusService.GetResolvedStatuses().ConvertStatusesToSql();
				var sqlCount = new Sql()
					.Select("Id")
					.From(TicketTableAlias)
					.Where($"StatusId IN ({statuses})");

				return scope.Database.Fetch<uSupportTicket>(sqlCount).Any();
			}
		}

		public override uSupportTicket Get(Guid id)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var sql = new Sql()
					.Select("*")
					.From(TicketTableAlias)
					.GetFullTicket()
					.Where($"{TicketTableAlias}.Id = UPPER('{id}')");
					
				var ticket = scope.Database.Fetch<uSupportTicket>(sql).FirstOrDefault();

				return ticket;
			}
		}

		public override uSupportTicket Update(uSupportTicketSchema dto)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				scope.Database.Update(dto);
				scope.Complete();
			}

			return Get(dto.Id);
		}

		public void ClearTicketCache()
		{
			_appCaches.RuntimeCache.ClearByRegex("uSupportPaged");
		}
	}
}
using NPoco;
using uSupport.Dtos;
using uSupport.Extensions;
using uSupport.Dtos.Tables;
using uSupport.Notifications;
using Umbraco.Cms.Core.Events;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Infrastructure.Scoping;
using static uSupport.Helpers.uSupportPageHelper;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketService : uSupportServiceBase<uSupportTicket, uSupportTicketSchema>, IuSupportTicketService
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;
		private readonly IuSupportTicketCommentService _uSupportTicketCommentService;
		private readonly IuSupportTicketHistoryService _uSupportTicketHistoryService;

		public uSupportTicketService(IEventAggregator eventAggregator,
			IScopeProvider scopeProvider,
			IScopeAccessor scopeAccessor,
			IuSupportTicketStatusService uSupportTicketStatusService,
			IuSupportTicketCommentService uSupportTicketCommentService,
			IuSupportTicketHistoryService uSupportTicketHistoryService) : base(TicketTableAlias, scopeProvider, scopeAccessor)
		{
			_eventAggregator = eventAggregator;
			_uSupportTicketStatusService = uSupportTicketStatusService;
			_uSupportTicketCommentService = uSupportTicketCommentService;
			_uSupportTicketHistoryService = uSupportTicketHistoryService;
		}

		public IEnumerable<uSupportTicket> GetAll()
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
								.Select("*")
								.From(TicketTableAlias)
								.OrderBy("LastUpdated DESC");

				return context.Scope.Database.Fetch<uSupportTicket>(sql);
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public uSupportPage<uSupportTicket> GetPagedActiveTickets(long page, string? searchTerm = null, uSupportSort? sort = null)
		{
			var context = GetScope();
			try
			{
				var statuses = _uSupportTicketStatusService.GetActiveStatuses().ConvertStatusesToSql();

				var sql = new Sql()
					.Select("*")
					.From(TicketTableAlias)
					.GetFullTicket()
					.Where($"{TicketTableAlias}.StatusId IN ({statuses})");

				var sqlCount = new Sql()
					.Select($"COUNT({TicketTableAlias}.Id)")
					.From(TicketTableAlias)
					.Where($"{TicketTableAlias}.StatusId IN ({statuses})");

				if (!string.IsNullOrWhiteSpace(searchTerm))
				{
					string searchPattern = $"{TicketTableAlias}.Title LIKE @0 OR {TicketTableAlias}.Summary LIKE @0 OR {TicketTableAlias}.ExternalTicketId LIKE @0";
					sql.Where(searchPattern, $"%{searchTerm}%");
					sqlCount.Where(searchPattern, $"%{searchTerm}%");
				}

				if(sort != null)
				{
					sql.OrderBy($"{sort.ColumnName} {(sort.Reverse ? "DESC" : "ASC")}");
				}
				else
				{
				 sql.OrderBy("LastUpdated DESC");
				}

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

		public uSupportPage<uSupportTicket> GetPagedResolvedTickets(long page, string? searchTerm = null, uSupportSort? sort = null)
		{
			var context = GetScope();
			try
			{
				var statuses = _uSupportTicketStatusService.GetResolvedStatuses().ConvertStatusesToSql();
				var sql = new Sql()
					.Select("*")
					.From(TicketTableAlias)
					.GetFullTicket()
					.Where($"{TicketTableAlias}.StatusId IN ({statuses})");

				var sqlCount = new Sql()
					.Select($"COUNT({TicketTableAlias}.Id)")
					.From(TicketTableAlias)
					.Where($"{TicketTableAlias}.StatusId IN ({statuses})");

				if (!string.IsNullOrWhiteSpace(searchTerm))
				{
					string searchPattern = $"{TicketTableAlias}.Title LIKE @0 OR {TicketTableAlias}.Summary LIKE @0 OR {TicketTableAlias}.ExternalTicketId LIKE @0";
					sql.Where(searchPattern, $"%{searchTerm}%");
					sqlCount.Where(searchPattern, $"%{searchTerm}%");
				}

				if (sort != null)
				{
					sql.OrderBy($"{sort.ColumnName} {(sort.Reverse ? "DESC" : "ASC")}");
				}
				else
				{
					sql.OrderBy("Resolved DESC");
				}

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

		public bool AnyResolvedTickets()
		{
			var context = GetScope();
			try
			{
				var statuses = _uSupportTicketStatusService.GetResolvedStatuses().ConvertStatusesToSql();
				var sqlCount = new Sql()
					.Select($"COUNT({TicketTableAlias}.Id)")
					.From(TicketTableAlias)
					.Where($"{TicketTableAlias}.StatusId IN ({statuses})");

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

		public override uSupportTicket Create(uSupportTicketSchema dto)
		{
			var ticket = base.Create(dto);

			_eventAggregator.Publish(new CreateTicketNotification(ticket));
			_eventAggregator.Publish(new TicketHistoryNotification(dto.AuthorId, null, ticket));

			return ticket;
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

			var ticket = Get(dto.Id);
			_eventAggregator.Publish(new UpdateTicketNotification(ticket));

			return ticket;
		}

		public override void Delete(Guid id)
		{
			var context = GetScope();

			try
			{
				var ticket = Get(id);

				base.Delete(id);

				_uSupportTicketCommentService.DeleteByTicketId(id);
				_uSupportTicketHistoryService.DeleteByTicketId(id);

				_eventAggregator.Publish(new DeleteTicketNotification(ticket));

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
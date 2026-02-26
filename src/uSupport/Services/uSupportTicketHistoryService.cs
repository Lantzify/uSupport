using NPoco;
using uSupport.Dtos;
using System.Text.Json;
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Services;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Core.Models.ContentEditing;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketHistoryService : uSupportServiceBase<uSupportTicketHistory, uSupportTicketHistorySchema>, IuSupportTicketHistoryService
	{
		private readonly IUserService _userService;
		private readonly IUmbracoMapper _umbracoMapper;

		public uSupportTicketHistoryService(IUserService userService,
			IScopeProvider scopeProvider,
			IScopeAccessor scopeAccessor,
			IUmbracoMapper umbracoMapper) : base(TicketHistoryTableAlias, scopeProvider, scopeAccessor)
		{
			_userService = userService;
			_umbracoMapper = umbracoMapper;
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
					var user = _userService.GetUserById(item.UserId);
					if(user != null)
						item.User = _umbracoMapper.Map<IUser, UserDisplay>(user);

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
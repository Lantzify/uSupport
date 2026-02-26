using NPoco;
using uSupport.Helpers;
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Services;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.ContentEditing;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketCommentService : uSupportServiceBase<uSupportTicketComment, uSupportTicketCommentSchema>, IuSupportTicketCommentService
	{
		private readonly IUserService _userService;
		private readonly IUmbracoMapper _umbracoMapper;

		public uSupportTicketCommentService(IUserService userService,
			IScopeProvider scopeProvider,
			IScopeAccessor scopeAccessor,
			IUmbracoMapper umbracoMapper) : base(TicketCommentTableAlias, scopeProvider, scopeAccessor)
		{
			_userService = userService;
			_umbracoMapper = umbracoMapper;
		}

		public IEnumerable<uSupportTicketComment> GetAll()
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
								.Select("*")
								.From(TicketCommentTableAlias);

				return context.Scope.Database.Fetch<uSupportTicketComment>(sql);
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public IEnumerable<uSupportTicketComment> GetCommentsFromTicketId(Guid ticketId)
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
					.Select("*")
					.From(TicketCommentTableAlias)
					.Where($"TicketId = UPPER('{ticketId}')")
					.OrderBy("Date DESC");

				var comments = context.Scope.Database.Query<uSupportTicketComment>(sql);

				List<uSupportTicketComment> commentDtos = new List<uSupportTicketComment>();

				foreach (var comment in comments.ToList())
				{
					var user = _userService.GetUserById(comment.UserId);

					comment.User = _umbracoMapper.Map<IUser, UserDisplay>(user);

					commentDtos.Add(comment);
				}

				return commentDtos;
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
				context.Scope.Database.Delete<uSupportTicketComment>($"WHERE {TicketCommentTableAlias}.TicketId = UPPER('{ticketId}')");

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
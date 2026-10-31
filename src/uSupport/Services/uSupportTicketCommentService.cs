using NPoco;
using uSupport.Dtos;
using uSupport.Dtos.Tables;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Infrastructure.Scoping;
using uSupport.Helpers;
using static uSupport.Helpers.uSupportPageHelper;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketCommentService : uSupportServiceBase<uSupportTicketComment, uSupportTicketCommentSchema>, IuSupportTicketCommentService
	{
		private readonly uSupportUserHelper _userHelper;

		public uSupportTicketCommentService(uSupportUserHelper userHelper,
			IScopeProvider scopeProvider,
			IScopeAccessor scopeAccessor) : base(TicketCommentTableAlias, scopeProvider, scopeAccessor)
		{
			_userHelper = userHelper;
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
					comment.User = _userHelper.GetUser(comment.UserId);

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

		public uSupportPage<uSupportTicketComment> GetPagedCommentsForTicket(Guid ticketId, long page)
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
					.Select("*")
					.From(TicketCommentTableAlias)
					.Where($"{TicketCommentTableAlias}.TicketId = UPPER('{ticketId}')")
					.OrderBy($"{TicketCommentTableAlias}.Date DESC");

				var sqlCount = new Sql()
					.Select($"COUNT({TicketCommentTableAlias}.Id)")
					.From(TicketCommentTableAlias)
					.Where($"{TicketCommentTableAlias}.TicketId = UPPER('{ticketId}')");

				var commentCount = context.Scope.Database.Fetch<int>(sqlCount).FirstOrDefault();
				var comments = context.Scope.Database.SkipTake<uSupportTicketComment>((page - 1) * PageSize, PageSize, sql);

				List<uSupportTicketComment> commentDtos = new List<uSupportTicketComment>();

				foreach (var comment in comments.ToList())
				{
					comment.User = _userHelper.GetUser(comment.UserId);

					commentDtos.Add(comment);
				}

				return MapPageToUSupportPage(commentDtos, commentCount, page, PageSize);
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
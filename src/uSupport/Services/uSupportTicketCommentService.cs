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

		public override uSupportTicketComment Get(Guid id)
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
							.Select("*")
							.From(TicketCommentTableAlias)
							.Where($"Id = UPPER('{id}')");

				return context.Scope.Database.Fetch<uSupportTicketCommentSchema>(sql).FirstOrDefault()?.ConvertSchemaToDto();

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
					.OrderBy("Date");

				var comments = context.Scope.Database.Query<uSupportTicketCommentSchema>(sql);

				List<uSupportTicketComment> commentDtos = new List<uSupportTicketComment>();

				foreach (var comment in comments.ToList())
				{
					var dto = comment.ConvertSchemaToDto();
					var user = _userService.GetUserById(dto.UserId);

					dto.User = _umbracoMapper.Map<IUser, UserDisplay>(user);

					commentDtos.Add(dto);
				}

				return commentDtos;
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}
	}
}
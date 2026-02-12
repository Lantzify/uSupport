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
		private static IScopeProvider _scopeProvider;
		private readonly IUmbracoMapper _umbracoMapper;


		public uSupportTicketCommentService(IUserService userService,
			IScopeProvider scopeProvider,
		 IUmbracoMapper umbracoMapper
		) : base(TicketCommentTableAlias, scopeProvider)
		{
			_userService = userService;
			_scopeProvider = scopeProvider;
			_umbracoMapper = umbracoMapper;
		}

		public override uSupportTicketComment Get(Guid id)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var sql = new Sql()
					.Select("*")
					.From(TicketCommentTableAlias)
					.Where($"Id = UPPER('{id}')");

				return scope.Database.Fetch<uSupportTicketCommentSchema>(sql).FirstOrDefault()?.ConvertSchemaToDto();
			}
		}

		public IEnumerable<uSupportTicketComment> GetCommentsFromTicketId(Guid ticketId)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var sql = new Sql()
					.Select("*")
					.From(TicketCommentTableAlias)
					.Where($"TicketId = UPPER('{ticketId}')")
					.OrderBy("Date");

				var comments = scope.Database.Query<uSupportTicketCommentSchema>(sql);

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
		}
	}
}
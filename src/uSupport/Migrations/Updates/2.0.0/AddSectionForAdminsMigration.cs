using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Services;
using static Umbraco.Cms.Core.Constants;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Infrastructure.Migrations;

namespace uSupport.Migrations.Updates._2._0._0
{
	public class AddSectionForAdminsMigration : uSupportMigrationBase
	{
		private readonly IUserService _userService;
		private readonly IScopeProvider _scopeProvider;
		private readonly IUmbracoContextFactory _umbracoContext;

		public AddSectionForAdminsMigration(IUserService userService,
			IScopeProvider scopeProvider,
			IUmbracoContextFactory umbracoContext,
			IMigrationContext context) : base(context)
		{
			_userService = userService;
			_scopeProvider = scopeProvider;
			_umbracoContext = umbracoContext;
		}

		protected override void DoMigrate()
		{
			using (UmbracoContextReference umbracoContextReference = _umbracoContext.EnsureUmbracoContext())
			{
				using (var scope = _scopeProvider.CreateScope())
				{
					var adminGroup = _userService.GetUserGroupByAlias(Security.AdminGroupAlias);
					adminGroup.AddAllowedSection(Constants.uSupportConstants.SectionAlias);

					_userService.Save(adminGroup);

					scope.Complete();
				}
			}
		}
	}
}
using uSupport.Constants;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace uSupport.Migrations.Updates._2._0._0
{
	public class AddSectionForAdminsMigration : AsyncMigrationBase
	{
		private readonly IUserGroupService _userGroupService;

		public AddSectionForAdminsMigration(IUserGroupService userGroupService, IMigrationContext context) : base(context)
		{
			_userGroupService = userGroupService;
		}

		protected override async Task MigrateAsync()
		{
			var adminGroup = await _userGroupService.GetAsync(Umbraco.Cms.Core.Constants.Security.AdminGroupAlias);
			if (adminGroup == null || adminGroup.AllowedSections.Contains(uSupportConstants.SectionAlias))
				return;

			adminGroup.AddAllowedSection(uSupportConstants.SectionAlias);
			await _userGroupService.UpdateAsync(adminGroup, Umbraco.Cms.Core.Constants.Security.SuperUserKey);
		}
	}
}

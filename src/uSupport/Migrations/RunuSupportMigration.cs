using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace uSupport.Migrations
{
	public class RunuSupportMigration : INotificationHandler<UmbracoApplicationStartingNotification>
	{
		private readonly IRuntimeState _runtimeState;
		private readonly IScopeProvider _scopeProvider;
		private readonly IKeyValueService _keyValueService;
		private readonly IMigrationPlanExecutor _migrationPlanExecutor;

		public RunuSupportMigration(IUserService userService,
			IRuntimeState runtimeState,
			IScopeProvider scopeProvider,
			IKeyValueService keyValueService,
			IUmbracoContextFactory umbracoContext,
			IMigrationPlanExecutor migrationPlanExecutor)
		{
			_runtimeState = runtimeState;
			_scopeProvider = scopeProvider;
			_keyValueService = keyValueService;
			_migrationPlanExecutor = migrationPlanExecutor;
		}

		public void Handle(UmbracoApplicationStartingNotification notification)
		{
			if (_runtimeState.Level < RuntimeLevel.Run)
				return;

			var upgrader = new Upgrader(new uSupportMigrationPlan());
			upgrader.Execute(
				_migrationPlanExecutor,
				_scopeProvider,
				_keyValueService);
		}
	}
}
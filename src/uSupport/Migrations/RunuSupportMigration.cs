using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace uSupport.Migrations
{
	public class RunuSupportMigration : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
	{
		private readonly IRuntimeState _runtimeState;
		private readonly ICoreScopeProvider _scopeProvider;
		private readonly IKeyValueService _keyValueService;
		private readonly IMigrationPlanExecutor _migrationPlanExecutor;

		public RunuSupportMigration(IRuntimeState runtimeState,
			ICoreScopeProvider scopeProvider,
			IKeyValueService keyValueService,
			IMigrationPlanExecutor migrationPlanExecutor)
		{
			_runtimeState = runtimeState;
			_scopeProvider = scopeProvider;
			_keyValueService = keyValueService;
			_migrationPlanExecutor = migrationPlanExecutor;
		}

		public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
		{
			if (_runtimeState.Level < RuntimeLevel.Run)
				return;

			var upgrader = new Upgrader(new uSupportMigrationPlan());
			await upgrader.ExecuteAsync(_migrationPlanExecutor, _scopeProvider, _keyValueService);
		}
	}
}

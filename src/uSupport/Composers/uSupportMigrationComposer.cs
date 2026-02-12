using uSupport.Migrations;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.DependencyInjection;

namespace uSupport.Composers
{
	public class uSupportMigrationComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.AddNotificationHandler<UmbracoApplicationStartingNotification, RunuSupportMigration>();
		}
	}
}
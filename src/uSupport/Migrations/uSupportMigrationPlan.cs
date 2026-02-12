using uSupport.Migrations.Create;
using uSupport.Migrations.Updates._1._2._0;
using uSupport.Migrations.Updates._2._0._0;
using Umbraco.Cms.Infrastructure.Migrations;

namespace uSupport.Migrations
{
	public class uSupportMigrationPlan : MigrationPlan
	{
		public uSupportMigrationPlan() : base("uSupport")
		{
			DefinePlan();
		}

		protected void DefinePlan()
		{
			From(string.Empty)
				.To<uSupportTicketCommentTable>("uSupport-ticket-comment")
				.To<uSupportTicketTypeTable>("uSupport-ticket-type")
				.To<uSupportTicketStatusTable>("uSupport-ticket-status")
				.To<uSupportTicketTable>("uSupport-ticket")
				.To<MakeExistingGuidIdsUpperCase>("uSupport-ticket-status-type-guid-update")
				.To<AddSectionForAdminsMigration>("uSupport-add-section-for-admin-update");
		}
	}
}
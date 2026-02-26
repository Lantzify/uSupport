using uSupport.Migrations.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

namespace uSupport.Migrations.Create
{
    public class uSupportTicketHistoryTable : uSupportMigrationBase
    {
		public uSupportTicketHistoryTable(IMigrationContext context) : base(context)
		{ }

		protected override void DoMigrate()
		{
			Create.Table<uSupportTicketHistorySchema>().Do();
		}
	}
}
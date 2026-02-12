using uSupport.Migrations.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

namespace uSupport.Migrations.Create
{
    public class uSupportTicketTable : uSupportMigrationBase
    {
		public uSupportTicketTable(IMigrationContext context) : base(context)
		{ }

		protected override void DoMigrate()
		{
			Create.Table<uSupportTicketSchema>().Do();
		}
	}
}
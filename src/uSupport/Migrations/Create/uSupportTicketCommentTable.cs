using uSupport.Migrations.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

namespace uSupport.Migrations.Create
{
    public class uSupportTicketCommentTable : uSupportMigrationBase
    {
		public uSupportTicketCommentTable(IMigrationContext context) : base(context)
		{ }

		protected override void DoMigrate()
		{
			Create.Table<uSupportTicketCommentSchema>().Do();
		}
	}
}
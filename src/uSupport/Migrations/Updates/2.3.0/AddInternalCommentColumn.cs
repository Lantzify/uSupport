using uSupport.Migrations.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Updates._2._3._0
{
	public class AddInternalCommentColumn : uSupportMigrationBase
	{
		public AddInternalCommentColumn(IMigrationContext context) : base(context) { }

		protected override void DoMigrate()
		{
			if (ColumnExists(TicketTableAlias, "InternalComment"))
				return;

			AddColumn<uSupportTicketSchema>("InternalComment");
		}
	}
}

using uSupport.Migrations.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Updates._2._3._0
{
	public class AddLastUpdatedToTickets : uSupportMigrationBase
	{
		public AddLastUpdatedToTickets(IMigrationContext context) : base(context)
		{ }

		protected override void DoMigrate()
		{
			if (ColumnExists(TicketTableAlias, "LastUpdated"))
				return;

			var colums = Context.SqlContext.SqlSyntax.GetColumnsInSchema(Context.Database);

			AddColumnIfNotExists<uSupportTicketSchema>(colums, "LastUpdated");

			Execute.Sql($"UPDATE {TicketTableAlias} SET LastUpdated = Submitted WHERE Resolved IS NULL").Do();
			Execute.Sql($"UPDATE {TicketTableAlias} SET LastUpdated = Resolved WHERE Resolved IS NOT NULL").Do();
		}
	}
}

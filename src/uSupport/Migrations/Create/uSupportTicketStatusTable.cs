using uSupport.Migrations.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Create
{
	public class uSupportTicketStatusTable : uSupportMigrationBase
	{
		public uSupportTicketStatusTable(IMigrationContext context) : base(context)
		{ }

		protected override void DoMigrate()
		{
			Create.Table<uSupportTicketStatusSchema>().Do();
			Insert.IntoTable(TicketStatusTableAlias)
				.Row(new
				{
					Id = NewTicketStatusGuid,
					Alias = "new",
					Name = "New",
					Default = true,
					Active = true,
					Color = "secondary",
					Order = 1
				})
				.Row(new
				{
					Id = InProgressTicketStatusGuid,
					Alias = "InProgress",
					Name = "In progress",
					Default = false,
					Active = true,
					Color = "gray",
					Order = 2
				})
				.Row(new
				{
					Id = AnsweredTicketStatusGuid,
					Alias = "answered",
					Name = "Answered",
					Default = false,
					Active = true,
					Color = "secondary",
					Order = 3
				})
				.Row(new
				{
					Id = ResolvedTicketStatusGuid,
					Alias = "resolved",
					Name = "Resolved",
					Default = false,
					Active = false,
					Color = "success",
					Order = 4
				})
				.Do();
		}
	}
}
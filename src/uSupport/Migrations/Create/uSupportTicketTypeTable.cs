using uSupport.Migrations.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Create
{
	public class uSupportTicketTypeTable : uSupportMigrationBase
	{
		public uSupportTicketTypeTable (IMigrationContext context) : base(context)
		{ }

		protected override void DoMigrate()
		{
			Create.Table<uSupportTicketTypeSchema>().Do();
			Insert.IntoTable(TicketTypeTableAlias)
				.Row(new {
					Id = PageErrorTicketTypeGuid,
					Alias = "pageError",
					Name = "Page error",				
					Color = "color-indigo",
					Icon = "icon-layout",
					Description = "A page isn''t working",
					PropertyName = "Page",
					PropertyDescription = "Select the page that you''re are having an issue with",
					PropertyId = 1046,
					PropertyView = "contentpicker",
					Order = 1

				}).Row(new {
					Id = SystemErrorTicketTypeGuid,
					Alias = "systemError",
					Name = "System error",					
					Color = "color-orange",
					Icon = "icon-server",
					Description = "Umbraco isn''t working",
					Order = 2
				}).Row(new {
					Id = GeneralQuestionTicketTypeGuid,
					Alias = "generalQuestion",
					Name = "General question",					
					Color = "color-blue",
					Icon = "icon-help-alt",
					Description = "A general question",
					Order = 3
				})
				.Do();
		}
	}
}
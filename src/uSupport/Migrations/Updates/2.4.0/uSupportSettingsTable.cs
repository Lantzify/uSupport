using uSupport.Migrations.Schemas;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Infrastructure.Migrations;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Updates._2._4._0
{
    public class uSupportSettingsTable : uSupportMigrationBase
    {
		private readonly IConfiguration _configuration;

		public uSupportSettingsTable(IMigrationContext context, IConfiguration configuration) : base(context)
		{
			_configuration = configuration;
		}

		protected override void DoMigrate()
		{
			Create.Table<uSupportSettingsSchema>().Do();

			var settings = new uSupportSettingsSchema();

			var settingsSection = _configuration.GetSection("uSupport:Settings:Tickets");
			if (settingsSection.Exists())
			{
				settingsSection.Bind(settings);
			}

			Database.Insert(SettingsTableAlias, "Id", false, settings);
		}
	}
}
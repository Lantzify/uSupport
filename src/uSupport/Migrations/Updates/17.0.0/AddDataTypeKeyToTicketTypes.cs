using NPoco;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using uSupport.Migrations.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Updates._17._0._0
{
	/// <summary>
	/// v13 stored the custom property as an int data type id plus an AngularJS view name.
	/// v17 references data types by key, so resolve the key once and keep the old columns untouched.
	/// </summary>
	public class AddDataTypeKeyToTicketTypes : uSupportMigrationBase
	{
		private readonly IIdKeyMap _idKeyMap;

		public AddDataTypeKeyToTicketTypes(IMigrationContext context, IIdKeyMap idKeyMap) : base(context)
		{
			_idKeyMap = idKeyMap;
		}

		protected override void DoMigrate()
		{
			if (!ColumnExists(TicketTypeTableAlias, "DataTypeKey"))
				AddColumn<uSupportTicketTypeSchema>("DataTypeKey");

			var types = Database.Fetch<uSupportTicketTypeSchema>(new Sql().Select("*").From(TicketTypeTableAlias));
			foreach (var type in types.Where(x => x.DataTypeKey == null && x.PropertyId != 0))
			{
				var key = _idKeyMap.GetKeyForId(type.PropertyId, UmbracoObjectTypes.DataType);
				if (key.Success)
					type.DataTypeKey = key.Result;
				else if (type.PropertyView == "contentpicker")
					type.DataTypeKey = Umbraco.Cms.Core.Constants.DataTypes.Guids.ContentPickerGuid;
				else
					continue;

				Database.Update(TicketTypeTableAlias, "Id", type, new[] { "DataTypeKey" });
			}
		}
	}
}

using NPoco;
using System.Text.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using uSupport.Migrations.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Updates._17._0._0
{
	/// <summary>
	/// v13 stored content picker values as an int node id or a document UDI.
	/// The v17 document picker reads a document key, so convert them once.
	/// </summary>
	public class MigrateContentPickerValues : uSupportMigrationBase
	{
		private readonly IIdKeyMap _idKeyMap;

		public MigrateContentPickerValues(IMigrationContext context, IIdKeyMap idKeyMap) : base(context)
		{
			_idKeyMap = idKeyMap;
		}

		protected override void DoMigrate()
		{
			// Left join: a type whose data type was deleted fell back to the default content picker in AddDataTypeKeyToTicketTypes
			var tickets = Database.Fetch<uSupportTicketSchema>(new Sql(
				$"SELECT t.Id, t.PropertyValue FROM {TicketTableAlias} t " +
				$"INNER JOIN {TicketTypeTableAlias} tt ON t.TypeId = tt.Id " +
				"LEFT JOIN umbracoDataType dt ON dt.nodeId = tt.PropertyId " +
				"WHERE tt.PropertyView = 'contentpicker' AND t.PropertyValue IS NOT NULL " +
				"AND (dt.propertyEditorAlias IS NULL OR dt.propertyEditorAlias = 'Umbraco.ContentPicker')"));

			foreach (var ticket in tickets)
			{
				var value = ToKeyJson(ticket.PropertyValue!, id =>
				{
					var key = _idKeyMap.GetKeyForId(id, UmbracoObjectTypes.Document);
					return key.Success ? key.Result : null;
				});

				if (value == null)
					continue;

				ticket.PropertyValue = value;
				Database.Update(TicketTableAlias, "Id", ticket, new[] { "PropertyValue" });
			}
		}

		/// <summary>
		/// Returns the v17 value (a JSON string holding the document key), or null when there is nothing to convert.
		/// </summary>
		public static string? ToKeyJson(string value, Func<int, Guid?> idToKey)
		{
			var raw = value.Trim().Trim('"');

			Guid? key = int.TryParse(raw, out var id) ? idToKey(id)
				: UdiParser.TryParse(raw, out Udi? udi) && udi is GuidUdi { EntityType: Umbraco.Cms.Core.Constants.UdiEntityType.Document } guidUdi ? guidUdi.Guid
				: null;

			return key == null ? null : JsonSerializer.Serialize(key.Value.ToString());
		}
	}
}

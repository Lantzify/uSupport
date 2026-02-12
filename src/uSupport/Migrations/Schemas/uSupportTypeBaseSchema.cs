using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace uSupport.Migrations.Schemas
{
	[PrimaryKey("Id")]
	public class uSupportTypeBaseSchema
	{
		[PrimaryKeyColumn(AutoIncrement = false)]
		[Column("Id")]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Column("Alias")]
		public string Alias { get; set; }

		[Column("Name")]
		public string Name { get; set; }

		[Column("Order")]
		public int Order { get; set; }

		[Column("Color")]
		[NullSetting]
		public string Color { get; set; }

		[Column("Icon")]
		[NullSetting]
		public string Icon { get; set; }
	}
}
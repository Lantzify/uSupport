using NPoco;
using Umbraco.Cms.Core.Models.ContentEditing;
using static uSupport.Constants.uSupportConstants;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace uSupport.Migrations.Schemas
{
	[ExplicitColumns, TableName(TicketCommentTableAlias)]
	[PrimaryKey("Id")]
	public class uSupportTicketCommentSchema
	{
		[PrimaryKeyColumn(AutoIncrement = false)]
		[Column("Id")]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Column("TicketId")]
		public Guid TicketId { get; set; }

		[Column("UserId")]
		public int UserId { get; set; }

		[ResultColumn]
		public UserDisplay User { get; set; }

		[Column("Date")]
		public DateTime Date { get; set; } = DateTime.Now;

		[Column("Comment")]
		[SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
		public string Comment { get; set; }
	}
}
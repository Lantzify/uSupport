using NPoco;
using uSupport.Dtos;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.ContentEditing;
using static uSupport.Constants.uSupportConstants;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace uSupport.Migrations.Schemas
{
	[ExplicitColumns, TableName(TicketHistoryTableAlias)]
	public class uSupportTicketHistorySchema
	{
		[PrimaryKeyColumn(AutoIncrement = false)]
		[Column("Id")]
		public Guid Id { get; set; } = Guid.NewGuid();
		
		[Column("TicketId")]
		public Guid TicketId { get; set; }


		[Column("ActionType")]
		public string ActionType { get; set; }

		[Column("UserId")]
		public int UserId { get; set; }

		[ResultColumn]
		public UserDisplay User { get; set; }

		[ResultColumn]
		public IEnumerable<uSupportChange> Changes { get; set; }

		[Column("ChangesJson")]
		[SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
		public string ChangesJson { get; set; }

		[Column("Date")]
		public DateTime Date { get; set; } = DateTime.Now;
	}
}
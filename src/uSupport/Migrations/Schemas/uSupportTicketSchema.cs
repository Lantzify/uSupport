using NPoco;
using uSupport.Dtos.Tables;
using static uSupport.Constants.uSupportConstants;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace uSupport.Migrations.Schemas
{
    [ExplicitColumns, TableName(TicketTableAlias)]
    [PrimaryKey("Id")]
    public class uSupportTicketSchema
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("Id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("Title")]
        public string Title { get; set; }

        [Column("Summary")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string Summary { get; set; }

        [Column("TypeId")]
        public Guid TypeId { get; set; }

        [ResultColumn]
        public uSupportTicketType Type { get; set; }

		[Column("StatusId")]
		public Guid StatusId { get; set; }
	
        [ResultColumn]
		public uSupportTicketStatus Status { get; set; }

		[Column("AuthorId")]
        public int AuthorId { get; set; }

        [Column("Submitted")]
        public DateTime Submitted { get; set; } = DateTime.Now;

        [Column("Resolved")]
        [NullSetting]
        public DateTime? Resolved { get; set; }

        [Column("LastUpdatedBy")]
        [NullSetting]
        public string LastUpdatedBy { get; set; }

        [Column("ExternalTicketId")]
        public string ExternalTicketId { get; set; }

        [Column("PropertyValue")]
        [NullSetting]
        public string PropertyValue { get; set; }
    }
}
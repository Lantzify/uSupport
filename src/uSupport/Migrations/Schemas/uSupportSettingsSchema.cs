using NPoco;
using static uSupport.Constants.uSupportConstants;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace uSupport.Migrations.Schemas
{
	[ExplicitColumns, TableName(SettingsTableAlias)]
	[PrimaryKey("Id")]
	public class uSupportSettingsSchema
	{
		[PrimaryKeyColumn(AutoIncrement = false)]
		[Column("Id")]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Column("SendEmailOnTicketCreated")]
		public bool SendEmailOnTicketCreated { get; set; } = true;

		[Column("SendEmailOnTicketComment")]
		public bool SendEmailOnTicketComment { get; set; } = true;

		[Column("TicketUpdateEmail")]
		[NullSetting]
		public string? TicketUpdateEmail { get; set; }

		[Column("EmailSubjectNewTicket")]
		public string EmailSubjectNewTicket { get; set; } = "A new ticket '{ExternalTicketId}' has been created";

		[Column("EmailSubjectUpdateTicket")]
		public string EmailSubjectUpdateTicket { get; set; } = "Ticket '{ExternalTicketId}' has been updated";

		[Column("EmailTemplateNewTicketPath")]
		public string EmailTemplateNewTicketPath { get; set; } = "/Views/Partials/uSupport/Emails/NewTicketEmail.cshtml";
		
		[Column("EmailTemplateUpdateTicketPath")]
		public string EmailTemplateUpdateTicketPath { get; set; } = "/Views/Partials/uSupport/Emails/UpdateTicketEmail.cshtml";
	}
}

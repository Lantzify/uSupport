using uSupport.Constants;

namespace uSupport.Dtos.Settings
{
	public class uSupportSettingsTicket
	{
		public const string Tickets = uSupportConstants.SectionAlias + ":Settings:Tickets";
		public bool SendEmailOnTicketCreated { get; set; } = true;
		public bool SendEmailOnTicketComment { get; set; } = true;
		public string TicketUpdateEmail { get; set; } = "None";
		public string EmailSubjectNewTicket { get; set; } = "A new ticket '{ExternalTicketId}' has been created";
		public string EmailSubjectUpdateTicket { get; set; } = "Ticket '{ExternalTicketId}' has been updated";
		public string EmailTemplateNewTicketPath { get; set; } = "/Views/Partials/uSupport/Emails/NewTicketEmail.cshtml";
		public string EmailTemplateUpdateTicketPath { get; set; } = "/Views/Partials/uSupport/Emails/UpdateTicketEmail.cshtml";
	}
}

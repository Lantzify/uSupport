namespace uSupport.Dtos.Tables
{
	public class uSupportSettings
	{
		public Guid Id { get; set; }
		public bool SendEmailOnTicketCreated { get; set; }
		public bool SendEmailOnTicketComment { get; set; }
		public string? TicketUpdateEmail { get; set; }
		public string EmailSubjectNewTicket { get; set; } = "A new ticket '{ExternalTicketId}' has been created";
		public string EmailSubjectUpdateTicket { get; set; } = "Ticket '{ExternalTicketId}' has been updated";
		public string EmailTemplateNewTicketPath { get; set; } = "/Views/Partials/uSupport/Emails/NewTicketEmail.cshtml";
		public string EmailTemplateUpdateTicketPath { get; set; } = "/Views/Partials/uSupport/Emails/UpdateTicketEmail.cshtml";
	}
}
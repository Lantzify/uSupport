using uSupport.Dtos.Tables;
using System.Threading.Tasks;

namespace uSupport.Services.Interfaces
{
	public interface IuSupportSettingsService
	{
		void SendEmail(string toAddress, string subject, string templateViewPath, object model);
		bool GetSendEmailOnTicketCreatedSetting();
        string GetTicketUpdateEmailSetting();
		string GetEmailSubjectNewTicket(uSupportTicket? ticket = null);
		string GetEmailSubjectUpdateTicket(uSupportTicket? ticket = null);
		string GetEmailTemplateNewTicketPath();
		string GetEmailTemplateUpdateTicketPath();
	}
}
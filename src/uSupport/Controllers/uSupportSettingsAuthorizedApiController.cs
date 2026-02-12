using Microsoft.AspNetCore.Mvc;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace uSupport.Controllers
{
	public class uSupportSettingsAuthorizedApiController : UmbracoAuthorizedApiController
	{
		private readonly IuSupportSettingsService _uSupportSettingsService;

		public uSupportSettingsAuthorizedApiController(IuSupportSettingsService uSupportSettingsService)
		{
			_uSupportSettingsService = uSupportSettingsService;
		}

		[HttpGet]
		public bool GetSendEmailOnTicketCreatedSetting() => _uSupportSettingsService.GetSendEmailOnTicketCreatedSetting();

        [HttpGet]
		public string GetTicketUpdateEmailSetting() => _uSupportSettingsService.GetTicketUpdateEmailSetting();

		[HttpGet]
		public string GetEmailSubjectNewTicket() => _uSupportSettingsService.GetEmailSubjectNewTicket();

		[HttpGet]
		public string GetEmailSubjectUpdateTicket() => _uSupportSettingsService.GetEmailSubjectUpdateTicket();

		[HttpGet]
		public string GetEmailTemplateNewTicketPath() => _uSupportSettingsService.GetEmailTemplateNewTicketPath();

		[HttpGet]
		public string GetEmailTemplateUpdateTicketPath() => _uSupportSettingsService.GetEmailTemplateUpdateTicketPath();

	}
}
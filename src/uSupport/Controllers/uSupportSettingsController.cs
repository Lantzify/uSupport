using uSupport.Constants;
using uSupport.Dtos.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Umbraco.Cms.Api.Management.Routing;

namespace uSupport.Controllers
{
	[VersionedApiBackOfficeRoute("usupport/settings")]
	[ApiExplorerSettings(GroupName = "Settings")]
	[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]
	public class uSupportSettingsController : uSupportControllerBase
	{
		private readonly IuSupportSettingsService _uSupportSettingsService;

		public uSupportSettingsController(IuSupportSettingsService uSupportSettingsService)
			=> _uSupportSettingsService = uSupportSettingsService;

		[HttpGet]
		[ProducesResponseType<uSupportSettings>(StatusCodes.Status200OK)]
		public IActionResult Get() => Ok(_uSupportSettingsService.GetSettings());

		[HttpPut]
		[ProducesResponseType<uSupportSettings>(StatusCodes.Status200OK)]
		public IActionResult Update(uSupportSettings model)
		{
			var existing = _uSupportSettingsService.GetSettings();

			return Ok(_uSupportSettingsService.Update(new uSupportSettingsSchema
			{
				Id = existing.Id,
				SendEmailOnTicketCreated = model.SendEmailOnTicketCreated,
				SendEmailOnTicketComment = model.SendEmailOnTicketComment,
				TicketUpdateEmail = model.TicketUpdateEmail,
				EmailSubjectNewTicket = model.EmailSubjectNewTicket,
				EmailSubjectUpdateTicket = model.EmailSubjectUpdateTicket,
				EmailTemplateNewTicketPath = model.EmailTemplateNewTicketPath,
				EmailTemplateUpdateTicketPath = model.EmailTemplateUpdateTicketPath
			}));
		}
	}
}

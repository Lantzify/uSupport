using uSupport.Dtos.Tables;
using Microsoft.AspNetCore.Mvc;
using uSupport.Migrations.Schemas;
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
		public uSupportSettings GetSettings() => _uSupportSettingsService.GetSettings();

        [HttpPost]
		public uSupportSettings UpdateSettings(uSupportSettingsSchema settings) => _uSupportSettingsService.Update(settings);
	}
}
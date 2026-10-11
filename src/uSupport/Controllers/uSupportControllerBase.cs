using Asp.Versioning;
using uSupport.Constants;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;

namespace uSupport.Controllers
{
	[ApiVersion("1.0")]
	[MapToApi(uSupportConstants.ApiName)]
	public abstract class uSupportControllerBase : ManagementApiControllerBase
	{ }
}

using uSupport.Constants;
using Umbraco.Cms.Core.Composing;
using Microsoft.AspNetCore.Authorization;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Security.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;

namespace uSupport.Composers
{
	public class uSupportAuthorizationComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.Services.AddSingleton<IAuthorizationHandler, uSupportSectionHandler>();
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy(uSupportConstants.uSupportSectionAccess, policy =>
				{
					policy.AuthenticationSchemes.Add(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
					policy.Requirements.Add(new uSupportSectionRequirement());
				});
			});
		}
	}

	public class uSupportSectionRequirement : IAuthorizationRequirement { }

	/// <summary>
	/// Same check as Umbraco's own section policies: the backoffice user must have the uSupport section.
	/// </summary>
	public class uSupportSectionHandler : AuthorizationHandler<uSupportSectionRequirement>
	{
		private readonly IAuthorizationHelper _authorizationHelper;

		public uSupportSectionHandler(IAuthorizationHelper authorizationHelper)
			=> _authorizationHelper = authorizationHelper;

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, uSupportSectionRequirement requirement)
		{
			if (_authorizationHelper.TryGetUmbracoUser(context.User, out var user)
				&& user.AllowedSections.Contains(uSupportConstants.SectionAlias))
				context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}

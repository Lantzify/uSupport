using uSupport.Constants;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Web.BackOffice.Authorization;

namespace uSupport.Composers
{
	public class uSupportAuthorizationComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("uSupportSectionAccess", policy =>
				{
					policy.Requirements.Add(new SectionRequirement(uSupportConstants.SectionAlias));
				});
			});
		}
	}
}

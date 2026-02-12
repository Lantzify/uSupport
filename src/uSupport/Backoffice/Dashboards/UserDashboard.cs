using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Dashboards;
using static Umbraco.Cms.Core.Constants;

namespace uSupport.Backoffice.Dashboards
{
	[Weight(-10)]
	public class UserDashboard : IDashboard
	{
		public string Alias => "uSupportUserDashboard";
		public string[] Sections => new[]
		{
			Applications.Content
		};
		public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
		public string View => "/App_Plugins/uSupport/components/dashboards/user/dashboard.html";
	}
}
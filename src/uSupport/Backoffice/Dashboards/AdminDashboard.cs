using uSupport.Constants;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Dashboards;

namespace uSupport.Backoffice.Dashboards
{
	[Weight(-10)]
	public class AdminDashboard : IDashboard
	{
		public string Alias => "uSupportAdminDashboard";
		public string[] Sections => new[]
		{
			uSupportConstants.SectionAlias
		};
		public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
		public string View => "/App_Plugins/uSupport/components/dashboards/admin/dashboard.html";
	}
}
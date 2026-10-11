import { USUPPORT_SECTION_ALIAS } from '../constants.js';

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'dashboard',
		alias: 'uSupport.Dashboard.Admin',
		name: 'uSupport Admin Dashboard',
		element: () => import('./admin-dashboard.element.js'),
		weight: 10,
		meta: { label: '#dashboardTabs_uSupportAdminDashboard', pathname: 'support' },
		conditions: [{ alias: 'Umb.Condition.SectionAlias', match: USUPPORT_SECTION_ALIAS }],
	},
	{
		type: 'dashboard',
		alias: 'uSupport.Dashboard.User',
		name: 'uSupport User Dashboard',
		element: () => import('./user-dashboard.element.js'),
		weight: -10,
		meta: { label: '#dashboardTabs_uSupportUserDashboard', pathname: 'support' },
		conditions: [{ alias: 'Umb.Condition.SectionAlias', match: 'Umb.Section.Content' }],
	},
];

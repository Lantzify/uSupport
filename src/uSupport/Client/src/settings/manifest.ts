import { USUPPORT_SETTINGS_ENTITY_TYPE, USUPPORT_SETTINGS_WORKSPACE_ALIAS } from '../constants.js';

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'workspace',
		kind: 'default',
		alias: USUPPORT_SETTINGS_WORKSPACE_ALIAS,
		name: 'uSupport Settings Workspace',
		meta: { entityType: USUPPORT_SETTINGS_ENTITY_TYPE, headline: '#uSupport_settings' },
	},
	{
		type: 'workspaceView',
		alias: 'uSupport.WorkspaceView.Settings.Email',
		name: 'uSupport Email Settings',
		element: () => import('./settings.view.element.js'),
		meta: { label: '#uSupport_email', pathname: 'email', icon: 'icon-message' },
		conditions: [{ alias: 'Umb.Condition.WorkspaceAlias', match: USUPPORT_SETTINGS_WORKSPACE_ALIAS }],
	},
];

import { USUPPORT_MENU_ALIAS } from '../constants.js';
import type { LookupConfig } from './lookup.js';

interface LookupManifestOptions {
	config: LookupConfig;
	prefix: string;
	icon: string;
	menuWeight: number;
	workspaceAlias: string;
	treeRepository: () => Promise<unknown>;
	workspaceContext: () => Promise<unknown>;
	overview: () => Promise<unknown>;
}

/** Tree in the uSupport menu, root overview, create/edit workspace and create/sort/delete actions. */
export const lookupManifests = (o: LookupManifestOptions): Array<UmbExtensionManifest> => {
	const { config, prefix } = o;
	const actions = () => import('./lookup-actions.js');
	const meta = { lookup: config.key };

	return [
		{ type: 'repository', alias: `${prefix}.Tree.Repository`, name: `${prefix} Tree Repository`, api: o.treeRepository as never },
		{ type: 'tree', kind: 'default', alias: `${prefix}.Tree`, name: `${prefix} Tree`, meta: { repositoryAlias: `${prefix}.Tree.Repository` } },
		{
			type: 'treeItem',
			kind: 'default',
			alias: `${prefix}.TreeItem`,
			name: `${prefix} Tree Item`,
			forEntityTypes: [config.rootEntityType, config.entityType],
		},
		{
			type: 'menuItem',
			kind: 'tree',
			alias: `${prefix}.MenuItem`,
			name: `${prefix} Menu Item`,
			weight: o.menuWeight,
			meta: { label: config.label, icon: o.icon, entityType: config.rootEntityType, menus: [USUPPORT_MENU_ALIAS], treeAlias: `${prefix}.Tree` },
		},

		// Root: overview table
		{
			type: 'workspace',
			kind: 'default',
			alias: `${o.workspaceAlias}.Root`,
			name: `${prefix} Root Workspace`,
			meta: { entityType: config.rootEntityType, headline: config.label },
		},
		{
			type: 'workspaceView',
			alias: `${prefix}.WorkspaceView.Overview`,
			name: `${prefix} Overview`,
			element: o.overview as never,
			meta: { label: '#uSupport_overview', pathname: 'overview', icon: o.icon },
			conditions: [{ alias: 'Umb.Condition.WorkspaceAlias', match: `${o.workspaceAlias}.Root` }],
		},

		// Item: create/edit
		{
			type: 'workspace',
			kind: 'routable',
			alias: o.workspaceAlias,
			name: `${prefix} Workspace`,
			api: o.workspaceContext as never,
			meta: { entityType: config.entityType },
		},

		{
			type: 'entityAction',
			kind: 'default',
			alias: `${prefix}.EntityAction.Create`,
			name: `${prefix} Create`,
			weight: 1200,
			api: () => actions().then((m) => ({ api: m.CreateLookupAction })),
			forEntityTypes: [config.rootEntityType],
			meta: { icon: 'icon-add', label: '#actions_create', ...meta } as never,
		},
		{
			type: 'entityAction',
			kind: 'default',
			alias: `${prefix}.EntityAction.Sort`,
			name: `${prefix} Sort`,
			weight: 1100,
			api: () => actions().then((m) => ({ api: m.SortLookupAction })),
			forEntityTypes: [config.rootEntityType],
			meta: { icon: 'icon-navigation-vertical', label: '#actions_sort', ...meta } as never,
		},
		{
			type: 'entityAction',
			kind: 'default',
			alias: `${prefix}.EntityAction.Delete`,
			name: `${prefix} Delete`,
			weight: 1000,
			api: () => actions().then((m) => ({ api: m.DeleteLookupAction })),
			forEntityTypes: [config.entityType],
			meta: { icon: 'icon-trash', label: '#actions_delete', ...meta } as never,
		},
	];
};

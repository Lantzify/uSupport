import {
	USUPPORT_MENU_ALIAS,
	USUPPORT_SECTION_ALIAS,
	USUPPORT_SECTION_PATHNAME,
	USUPPORT_SETTINGS_ENTITY_TYPE,
	USUPPORT_TICKETS_ROOT_ENTITY_TYPE,
} from '../constants.js';

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'section',
		alias: USUPPORT_SECTION_ALIAS,
		name: 'uSupport Section',
		weight: 10,
		meta: {
			label: '#sections_uSupport',
			pathname: USUPPORT_SECTION_PATHNAME,
		},
	},
	{
		type: 'menu',
		alias: USUPPORT_MENU_ALIAS,
		name: 'uSupport Menu',
	},
	{
		type: 'sectionSidebarApp',
		kind: 'menu',
		alias: 'uSupport.SectionSidebarApp',
		name: 'uSupport Sidebar Menu',
		meta: {
			label: '#treeHeaders_uSupportGroup',
			menu: USUPPORT_MENU_ALIAS,
		},
		conditions: [{ alias: 'Umb.Condition.SectionAlias', match: USUPPORT_SECTION_ALIAS }],
	},
	{
		type: 'menuItem',
		alias: 'uSupport.MenuItem.Tickets',
		name: 'uSupport Tickets Menu Item',
		weight: 400,
		meta: {
			label: '#uSupport_tickets',
			icon: 'icon-inbox-full',
			entityType: USUPPORT_TICKETS_ROOT_ENTITY_TYPE,
			menus: [USUPPORT_MENU_ALIAS],
		},
	},
	{
		type: 'menuItem',
		alias: 'uSupport.MenuItem.Settings',
		name: 'uSupport Settings Menu Item',
		weight: 100,
		meta: {
			label: '#uSupport_settings',
			icon: 'icon-settings',
			entityType: USUPPORT_SETTINGS_ENTITY_TYPE,
			menus: [USUPPORT_MENU_ALIAS],
		},
	},
];

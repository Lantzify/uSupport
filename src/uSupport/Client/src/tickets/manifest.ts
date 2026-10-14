import {
	USUPPORT_TICKETS_ROOT_ENTITY_TYPE,
	USUPPORT_TICKETS_WORKSPACE_ALIAS,
	USUPPORT_TICKET_ENTITY_TYPE,
	USUPPORT_TICKET_WORKSPACE_ALIAS,
} from '../constants.js';

const inWorkspace = (alias: string) => [{ alias: 'Umb.Condition.WorkspaceAlias', match: alias }];

export const manifests: Array<UmbExtensionManifest> = [
	// Overview of all tickets (the "Tickets" menu item)
	{
		type: 'workspace',
		kind: 'default',
		alias: USUPPORT_TICKETS_WORKSPACE_ALIAS,
		name: 'uSupport Tickets Workspace',
		meta: { entityType: USUPPORT_TICKETS_ROOT_ENTITY_TYPE, headline: '#uSupport_tickets' },
	},
	{
		type: 'workspaceView',
		alias: 'uSupport.WorkspaceView.Tickets.Overview',
		name: 'uSupport Tickets Overview',
		element: () => import('./overview/overview.view.element.js'),
		meta: { label: '#uSupport_overview', pathname: 'overview', icon: 'icon-inbox-full' },
		conditions: inWorkspace(USUPPORT_TICKETS_WORKSPACE_ALIAS),
	},

	// A single ticket. Other packages can add tabs with a workspaceView for this workspace alias.
	{
		type: 'workspace',
		kind: 'routable',
		alias: USUPPORT_TICKET_WORKSPACE_ALIAS,
		name: 'uSupport Ticket Workspace',
		api: () => import('./workspace/ticket-workspace.context.js'),
		meta: { entityType: USUPPORT_TICKET_ENTITY_TYPE },
	},
	{
		type: 'workspaceView',
		alias: 'uSupport.WorkspaceView.Ticket.Ticket',
		name: 'uSupport Ticket View',
		element: () => import('./workspace/views/ticket.view.element.js'),
		weight: 200,
		meta: { label: '#uSupport_ticket', pathname: 'ticket', icon: 'icon-ticket' },
		conditions: inWorkspace(USUPPORT_TICKET_WORKSPACE_ALIAS),
	},
	{
		type: 'workspaceView',
		alias: 'uSupport.WorkspaceView.Ticket.History',
		name: 'uSupport Ticket History View',
		element: () => import('./workspace/views/history.view.element.js'),
		weight: 100,
		meta: { label: '#uSupport_history', pathname: 'history', icon: 'icon-history' },
		conditions: inWorkspace(USUPPORT_TICKET_WORKSPACE_ALIAS),
	},

	{
		type: 'entityAction',
		kind: 'default',
		alias: 'uSupport.EntityAction.Ticket.Delete',
		name: 'uSupport Delete Ticket',
		weight: 1000,
		api: () => import('./entity-actions/delete-ticket.action.js'),
		forEntityTypes: [USUPPORT_TICKET_ENTITY_TYPE],
		meta: { icon: 'icon-trash', label: '#actions_delete' },
	},
	{
		type: 'modal',
		alias: 'uSupport.Modal.CreateTicket',
		name: 'uSupport Create Ticket Modal',
		element: () => import('./modals/create-ticket.modal.element.js'),
	},
	{
		type: 'modal',
		alias: 'uSupport.Modal.OpenTicket',
		name: 'uSupport Open Ticket Modal',
		element: () => import('./modals/open-ticket.modal.element.js'),
	},
];

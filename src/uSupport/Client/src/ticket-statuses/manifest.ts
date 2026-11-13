import { USUPPORT_TICKET_STATUS_WORKSPACE_ALIAS } from '../constants.js';
import { LOOKUPS } from '../shared/lookup.js';
import { lookupManifests } from '../shared/lookup.manifests.js';

export const manifests = lookupManifests({
	config: LOOKUPS.status,
	prefix: 'uSupport.TicketStatus',
	icon: 'icon-file-cabinet',
	menuWeight: 200,
	workspaceAlias: USUPPORT_TICKET_STATUS_WORKSPACE_ALIAS,
	treeRepository: () => import('./tree.repository.js'),
	workspaceContext: () => import('./ticket-status-workspace.context.js'),
	overview: () => import('../shared/lookup-overview.element.js').then((m) => ({ element: m.USupportTicketStatusesOverviewElement })),
});

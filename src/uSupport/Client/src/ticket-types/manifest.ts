import { USUPPORT_TICKET_TYPE_WORKSPACE_ALIAS } from '../constants.js';
import { LOOKUPS } from '../shared/lookup.js';
import { lookupManifests } from '../shared/lookup.manifests.js';

export const manifests = lookupManifests({
	config: LOOKUPS.type,
	prefix: 'uSupport.TicketType',
	icon: 'icon-ticket',
	menuWeight: 300,
	workspaceAlias: USUPPORT_TICKET_TYPE_WORKSPACE_ALIAS,
	treeRepository: () => import('./tree.repository.js'),
	workspaceContext: () => import('./ticket-type-workspace.context.js'),
	overview: () => import('../shared/lookup-overview.element.js').then((m) => ({ element: m.USupportTicketTypesOverviewElement })),
});

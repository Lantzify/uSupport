import type { TicketStatus } from '../repository/usupport.repository.js';
import { USUPPORT_TICKET_STATUS_WORKSPACE_ALIAS } from '../constants.js';
import { USupportLookupWorkspaceContext } from '../shared/lookup-workspace.context.js';
import { LOOKUPS } from '../shared/lookup.js';

export class USupportTicketStatusWorkspaceContext extends USupportLookupWorkspaceContext<TicketStatus> {
	readonly workspaceAlias = USUPPORT_TICKET_STATUS_WORKSPACE_ALIAS;
	protected readonly config = LOOKUPS.status;
	protected readonly editor = () => import('./ticket-status-editor.element.js');

	protected readonly empty = (): TicketStatus => ({
		id: '',
		name: '',
		alias: '',
		icon: 'icon-tag',
		color: 'secondary',
		order: 0,
		active: true,
		default: false,
	});
	protected readonly get = (id: string) => this.repository.getStatus(id);
	protected readonly create = (model: TicketStatus) => this.repository.createStatus(model);
	protected readonly update = (model: TicketStatus) => this.repository.updateStatus(model.id, model);
}

export { USupportTicketStatusWorkspaceContext as api };

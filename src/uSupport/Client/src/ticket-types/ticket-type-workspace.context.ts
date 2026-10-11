import type { TicketType } from '../repository/usupport.repository.js';
import { USUPPORT_TICKET_TYPE_WORKSPACE_ALIAS } from '../constants.js';
import { USupportLookupWorkspaceContext } from '../shared/lookup-workspace.context.js';
import { LOOKUPS } from '../shared/lookup.js';

export class USupportTicketTypeWorkspaceContext extends USupportLookupWorkspaceContext<TicketType> {
	readonly workspaceAlias = USUPPORT_TICKET_TYPE_WORKSPACE_ALIAS;
	protected readonly config = LOOKUPS.type;
	protected readonly editor = () => import('./ticket-type-editor.element.js');

	protected empty = (): TicketType => ({ id: '', name: '', alias: '', icon: 'icon-ticket', order: 0, propertyId: 0 });
	protected get = (id: string) => this.repository.getType(id);
	protected create = (model: TicketType) => this.repository.createType(model);
	protected update = (model: TicketType) => this.repository.updateType(model.id, model);
}

export { USupportTicketTypeWorkspaceContext as api };

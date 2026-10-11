import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import { UmbObjectState } from '@umbraco-cms/backoffice/observable-api';
import {
	UMB_WORKSPACE_CONTEXT,
	UmbWorkspaceRouteManager,
	type UmbRoutableWorkspaceContext,
	type UmbWorkspaceContext,
} from '@umbraco-cms/backoffice/workspace';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { UmbLocalizationController } from '@umbraco-cms/backoffice/localization-api';
import { UmbEntityContext } from '@umbraco-cms/backoffice/entity';
import { USupportRepository, type Ticket, type TicketStatus, type TicketType } from '../../repository/usupport.repository.js';
import { USUPPORT_TICKET_ENTITY_TYPE, USUPPORT_TICKET_WORKSPACE_ALIAS } from '../../constants.js';

export interface TicketDraft {
	typeId: string;
	statusId: string;
	internalComment?: string | null;
}

export class USupportTicketWorkspaceContext extends UmbContextBase implements UmbRoutableWorkspaceContext {
	readonly workspaceAlias = USUPPORT_TICKET_WORKSPACE_ALIAS;
	readonly routes = new UmbWorkspaceRouteManager(this);

	#repository = new USupportRepository(this);
	#localize = new UmbLocalizationController(this);

	#ticket = new UmbObjectState<Ticket | undefined>(undefined);
	readonly ticket = this.#ticket.asObservable();

	/** Entity type + unique for Umbraco's workspace entity action menu ("..."). */
	#entityContext = new UmbEntityContext(this);
	readonly unique = this.#entityContext.unique;

	#draft = new UmbObjectState<TicketDraft | undefined>(undefined);
	readonly draft = this.#draft.asObservable();

	#types = new UmbObjectState<TicketType[]>([]);
	readonly types = this.#types.asObservable();

	#statuses = new UmbObjectState<TicketStatus[]>([]);
	readonly statuses = this.#statuses.asObservable();

	constructor(host: UmbControllerHost) {
		super(host, UMB_WORKSPACE_CONTEXT);
		this.#entityContext.setEntityType(USUPPORT_TICKET_ENTITY_TYPE);
		this.observe(this.ticket, (ticket) => this.#entityContext.setUnique(ticket?.id ?? null));

		this.routes.setRoutes([
			{
				path: 'edit/:id',
				component: () => import('./ticket-editor.element.js'),
				setup: (_component, info) => this.load(info.match.params.id),
			},
		]);
	}

	getEntityType = () => USUPPORT_TICKET_ENTITY_TYPE;
	getUnique = () => this.#ticket.getValue()?.id;
	getTicket = () => this.#ticket.getValue();
	getRepository = () => this.#repository;

	async load(id: string) {
		const [ticket, types, statuses] = await Promise.all([
			this.#repository.getTicket(id),
			this.#repository.getTypes(),
			this.#repository.getStatuses(),
		]);
		this.#types.setValue(types ?? []);
		this.#statuses.setValue(statuses ?? []);
		this.#setTicket(ticket);
	}

	updateDraft(partial: Partial<TicketDraft>) {
		this.#draft.update(partial);
	}

	async save(sendEmail: boolean) {
		const ticket = this.#ticket.getValue();
		const draft = this.#draft.getValue();
		if (!ticket || !draft) return;

		const updated = await this.#repository.updateTicket(ticket.id, { ...draft, sendEmail });
		if (!updated) return;

		this.#setTicket(updated);
		(await this.getContext(UMB_NOTIFICATION_CONTEXT))?.peek('positive', {
			data: { message: this.#localize.term(sendEmail ? 'uSupport_ticketSavedAndSent' : 'uSupport_ticketSaved') },
		});
	}

	#setTicket(ticket?: Ticket) {
		this.#ticket.setValue(ticket);
		this.#draft.setValue(
			ticket ? { typeId: ticket.typeId, statusId: ticket.statusId, internalComment: ticket.internalComment } : undefined,
		);
	}
}

export const USUPPORT_TICKET_WORKSPACE_CONTEXT = new UmbContextToken<UmbWorkspaceContext, USupportTicketWorkspaceContext>(
	'UmbWorkspaceContext',
	undefined,
	(context): context is USupportTicketWorkspaceContext => context.workspaceAlias === USUPPORT_TICKET_WORKSPACE_ALIAS,
);

export { USupportTicketWorkspaceContext as api };

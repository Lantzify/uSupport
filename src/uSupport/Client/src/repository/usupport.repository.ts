import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import {
	SettingsService,
	TicketCommentService,
	TicketService,
	TicketStatusService,
	TicketTypeService,
	type CreateTicketRequestModel,
	type UpdateTicketRequestModel,
	type USupportSettingsModel,
	type USupportTicketStatusModel,
	type USupportTicketTypeModel,
} from '../api/index.js';

// The generated client is only touched here; the rest of the package uses these names.
export type {
	USupportTicketModel as Ticket,
	USupportTicketTypeModel as TicketType,
	USupportTicketStatusModel as TicketStatus,
	USupportTicketCommentModel as TicketComment,
	USupportTicketHistoryModel as TicketHistory,
	USupportSettingsModel as Settings,
	USupportUserModel as SupportUser,
	PreviewDeleteItemDtoModel as DependentTicket,
} from '../api/index.js';

export type TicketList = 'active' | 'resolved';
export interface TicketQuery {
	page: number;
	searchTerm?: string;
	sortColumn?: string;
	sortDescending?: boolean;
}

export class USupportRepository extends UmbControllerBase {
	constructor(host: UmbControllerHost) {
		super(host);
	}

	async #run<T>(promise: Promise<{ data?: T; error?: unknown }>) {
		const { data, error } = await tryExecute(this, promise);
		return error ? undefined : data;
	}

	/** Deletes return no body, so report success as a boolean. */
	async #delete(promise: Promise<unknown>) {
		const { error } = await tryExecute(this, promise);
		return !error;
	}

	// Tickets
	getTickets = (list: TicketList, query: TicketQuery) =>
		this.#run(list === 'active' ? TicketService.getUsupportTicketActive({ query }) : TicketService.getUsupportTicketResolved({ query }));
	getTicket = (id: string) => this.#run(TicketService.getUsupportTicketById({ path: { id } }));
	createTicket = (body: CreateTicketRequestModel) => this.#run(TicketService.postUsupportTicket({ body }));
	updateTicket = (id: string, body: UpdateTicketRequestModel) => this.#run(TicketService.putUsupportTicketById({ path: { id }, body }));
	deleteTicket = (id: string) => this.#delete(TicketService.deleteUsupportTicketById({ path: { id } }));
	getHistory = (id: string, page: number) => this.#run(TicketService.getUsupportTicketByIdHistory({ path: { id }, query: { page } }));

	// Comments
	getComments = (ticketId: string, page: number) =>
		this.#run(TicketCommentService.getUsupportTicketCommentTicketByTicketId({ path: { ticketId }, query: { page } }));
	createComment = (ticketId: string, comment: string) =>
		this.#run(TicketCommentService.postUsupportTicketComment({ body: { ticketId, comment } }));

	// Ticket types
	getTypes = () => this.#run(TicketTypeService.getUsupportTicketType());
	getType = (id: string) => this.#run(TicketTypeService.getUsupportTicketTypeById({ path: { id } }));
	createType = (body: USupportTicketTypeModel) => this.#run(TicketTypeService.postUsupportTicketType({ body }));
	updateType = (id: string, body: USupportTicketTypeModel) => this.#run(TicketTypeService.putUsupportTicketTypeById({ path: { id }, body }));
	deleteType = (id: string) => this.#delete(TicketTypeService.deleteUsupportTicketTypeById({ path: { id } }));
	getTypeDependents = (id: string) => this.#run(TicketTypeService.getUsupportTicketTypeByIdDependentTickets({ path: { id } }));
	sortTypes = (ids: string[]) => this.#run(TicketTypeService.putUsupportTicketTypeSort({ body: ids }));

	// Ticket statuses
	getStatuses = () => this.#run(TicketStatusService.getUsupportTicketStatus());
	getStatus = (id: string) => this.#run(TicketStatusService.getUsupportTicketStatusById({ path: { id } }));
	createStatus = (body: USupportTicketStatusModel) => this.#run(TicketStatusService.postUsupportTicketStatus({ body }));
	updateStatus = (id: string, body: USupportTicketStatusModel) => this.#run(TicketStatusService.putUsupportTicketStatusById({ path: { id }, body }));
	deleteStatus = (id: string) => this.#delete(TicketStatusService.deleteUsupportTicketStatusById({ path: { id } }));
	getStatusDependents = (id: string) => this.#run(TicketStatusService.getUsupportTicketStatusByIdDependentTickets({ path: { id } }));
	sortStatuses = (ids: string[]) => this.#run(TicketStatusService.putUsupportTicketStatusSort({ body: ids }));

	// Settings
	getSettings = () => this.#run(SettingsService.getUsupportSettings());
	updateSettings = (body: USupportSettingsModel) => this.#run(SettingsService.putUsupportSettings({ body }));
}

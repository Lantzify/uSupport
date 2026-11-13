import { css, customElement, html, nothing, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { Ticket, TicketHistory, TicketStatus, TicketType } from '../../../repository/usupport.repository.js';
import { USUPPORT_TICKET_WORKSPACE_CONTEXT, type USupportTicketWorkspaceContext } from '../ticket-workspace.context.js';
import { ticketDetails, ticketLayoutStyles } from '../../components/ticket-details.js';
import { formatDate } from '../../../shared/display.js';

const ACTION_COLOR: Record<string, string> = {
	Created: 'default',
	Updated: 'positive',
	Comment: 'positive',
	Resolved: 'positive',
	SentEmail: 'warning',
};

@customElement('usupport-ticket-history-view')
export class USupportTicketHistoryViewElement extends UmbLitElement {
	@state() private _ticket?: Ticket;
	@state() private _history: TicketHistory[] = [];
	@state() private _page = 1;
	@state() private _totalPages = 1;
	@state() private _types: TicketType[] = [];
	@state() private _statuses: TicketStatus[] = [];

	#context?: USupportTicketWorkspaceContext;

	constructor() {
		super();
		this.consumeContext(USUPPORT_TICKET_WORKSPACE_CONTEXT, (context) => {
			this.#context = context;
			this.observe(context?.types, (types) => (this._types = types ?? []));
			this.observe(context?.statuses, (statuses) => (this._statuses = statuses ?? []));
			this.observe(context?.ticket, (ticket) => {
				this._ticket = ticket;
				this._page = 1;
				this.#load();
			});
		});
	}

	async #load() {
		if (!this._ticket) return;
		const data = await this.#context?.getRepository().getHistory(this._ticket.id, this._page);
		this._history = data?.items ?? [];
		this._totalPages = Math.max(1, data?.totalPages ?? 1);
	}

	#name = (items: Array<{ id: string; name: string }>, id?: string | null) =>
		items.find((x) => x.id.toLowerCase() === id?.toLowerCase())?.name ?? id ?? '';

	#diff = (label: string, oldValue?: string | null, newValue?: string | null) =>
		html`<div>${this.localize.term(label)}: <s class="old">${oldValue}</s> <span class="new">${newValue}</span></div>`;

	#renderChange(item: TicketHistory) {
		return (item.changes ?? []).map((change) => {
			switch (change.field) {
				case 'Email':
					return html`<div>${this.localize.term('uSupport_emailTo')} ${change.new}</div>`;
				case 'Status':
					return this.#diff('uSupport_status', this.#name(this._statuses, change.old), this.#name(this._statuses, change.new));
				case 'Type':
					return this.#diff('uSupport_type', this.#name(this._types, change.old), this.#name(this._types, change.new));
				case 'InternalComment':
					return this.#diff('uSupport_internalComment', change.old, change.new);
				default:
					return nothing;
			}
		});
	}

	render() {
		if (!this._ticket) return nothing;

		return html`
			<div class="layout">
				<uui-box headline=${this.localize.term('uSupport_history')}>
					${this._history.length
						? html`<umb-history-list>
								${this._history.map(
									(item) => html`
										<umb-history-item .name=${item.user?.name ?? 'uSupport'} .detail=${formatDate(item.date)}>
											<umb-user-avatar slot="avatar" .name=${item.user?.name} .imgUrls=${item.user?.avatarUrls ?? []}></umb-user-avatar>
											<div class="log-type">
												<uui-tag color=${ACTION_COLOR[item.actionType] ?? 'default'}>${item.actionType}</uui-tag>
												<div>${this.#renderChange(item)}</div>
											</div>
										</umb-history-item>
									`,
								)}
							</umb-history-list>`
						: this.localize.term('content_noItemsToShow')}
					${this._totalPages > 1
						? html`<uui-pagination
								.current=${this._page}
								firstlabel=${this.localize.term('general_first')}
								previouslabel=${this.localize.term('general_previous')}
								nextlabel=${this.localize.term('general_next')}
								lastlabel=${this.localize.term('general_last')}
								.total=${this._totalPages}
								@change=${(e: Event) => {
									this._page = (e.target as HTMLElement & { current: number }).current;
									this.#load();
								}}></uui-pagination>`
						: nothing}
				</uui-box>
				<div>${ticketDetails(this._ticket, this.localize)}</div>
			</div>
		`;
	}

	static styles = [
		UmbTextStyles,
		ticketLayoutStyles,
		css`
			.old {
				color: var(--uui-color-danger);
			}
			.new {
				color: var(--uui-color-positive);
			}
			.log-type {
				display: grid;
				grid-template-columns: var(--uui-size-40) auto;
				gap: var(--uui-size-layout-1);
			}
			.log-type uui-tag {
				justify-self: center;
				height: fit-content;
				margin-top: auto;
				margin-bottom: auto;
			}
			uui-pagination {
				display: flex;
				justify-content: center;
				margin-top: var(--uui-size-layout-1);
			}
		`,
	];
}

export default USupportTicketHistoryViewElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-ticket-history-view': USupportTicketHistoryViewElement;
	}
}

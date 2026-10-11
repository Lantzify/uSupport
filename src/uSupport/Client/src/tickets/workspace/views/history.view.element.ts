import { css, customElement, html, nothing, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { Ticket, TicketHistory, TicketStatus, TicketType } from '../../../repository/usupport.repository.js';
import { USUPPORT_TICKET_WORKSPACE_CONTEXT, type USupportTicketWorkspaceContext } from '../ticket-workspace.context.js';
import { ticketDetails, ticketLayoutStyles } from '../../ticket-details.js';
import { formatDate } from '../../../shared/display.js';

const ACTION_COLOR: Record<string, string> = {
	Created: 'positive',
	Updated: 'default',
	Comment: 'default',
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

	#renderChange(item: TicketHistory) {
		return (item.changes ?? []).map((change) => {
			switch (change.field) {
				case 'Email':
					return html`<div>${this.localize.term('uSupport_emailTo')} ${change.new}</div>`;
				case 'Status':
					return html`<div>
						${this.localize.term('uSupport_status')}: ${this.#name(this._statuses, change.old)} → ${this.#name(this._statuses, change.new)}
					</div>`;
				case 'Type':
					return html`<div>
						${this.localize.term('uSupport_type')}: ${this.#name(this._types, change.old)} → ${this.#name(this._types, change.new)}
					</div>`;
				case 'InternalComment':
					return html`<div>${this.localize.term('uSupport_internalCommentUpdated')}</div>`;
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
					${this._history.map(
						(item) => html`
							<div class="item">
								<uui-avatar name=${item.user?.name ?? 'uSupport'} img-src=${item.user?.avatarUrls?.[0] ?? ''}></uui-avatar>
								<div>
									<uui-tag color=${ACTION_COLOR[item.actionType] ?? 'default'}>${item.actionType}</uui-tag>
									<strong>${item.user?.name}</strong>
									<small>${formatDate(item.date)}</small>
									${this.#renderChange(item)}
								</div>
							</div>
						`,
					)}
					${this._totalPages > 1
						? html`<uui-pagination
								.current=${this._page}
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
			.item {
				display: flex;
				gap: var(--uui-size-space-4);
				padding: var(--uui-size-space-3) 0;
				border-bottom: 1px solid var(--uui-color-border);
			}
			small {
				color: var(--uui-color-text-alt);
				margin-left: var(--uui-size-space-2);
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

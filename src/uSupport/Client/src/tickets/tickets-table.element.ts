import { css, customElement, html, nothing, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UMB_CURRENT_USER_CONTEXT } from '@umbraco-cms/backoffice/current-user';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { USupportRepository, type Ticket, type TicketList } from '../repository/usupport.repository.js';
import { USUPPORT_SECTION_ALIAS, USUPPORT_TICKET_ENTITY_TYPE, workspacePath } from '../constants.js';
import { formatDate, statusTag, typeIcon } from '../shared/display.js';
import { USUPPORT_OPEN_TICKET_MODAL } from './modals/tokens.js';

interface Column {
	key: string;
	label: string;
	render: (ticket: Ticket) => unknown;
}

const truncate = (value: string, length: number) => (value.length > length ? `${value.slice(0, length)}…` : value);

/** Paged, searchable, server-sorted list of active or resolved tickets. */
@customElement('usupport-tickets-table')
export class USupportTicketsTableElement extends UmbLitElement {
	@property()
	list: TicketList = 'active';

	@state() private _tickets: Ticket[] = [];
	@state() private _page = 1;
	@state() private _totalPages = 1;
	@state() private _search = '';
	@state() private _sortColumn = '';
	@state() private _sortDescending = true;
	@state() private _loading = false;
	@state() private _hasSection = false;

	#repository = new USupportRepository(this);
	#searchTimer?: number;

	constructor() {
		super();
		this.consumeContext(UMB_CURRENT_USER_CONTEXT, (context) => {
			this.observe(context?.allowedSections, (sections) => {
				this._hasSection = sections?.includes(USUPPORT_SECTION_ALIAS) ?? false;
			});
		});
	}

	connectedCallback() {
		super.connectedCallback();
		this.load();
	}

	get #columns(): Column[] {
		return [
			{ key: 'StatusId', label: 'uSupport_status', render: (t) => statusTag(t.status) },
			{ key: 'TypeId', label: 'uSupport_type', render: (t) => typeIcon(t.type) },
			{ key: 'ExternalTicketId', label: 'uSupport_ticketId', render: (t) => t.externalTicketId },
			{ key: 'Title', label: 'uSupport_title', render: (t) => truncate(t.title, 30) },
			...(this.list === 'resolved'
				? [
						{ key: 'Submitted', label: 'uSupport_submitted', render: (t: Ticket) => formatDate(t.submitted) },
						{ key: 'Resolved', label: 'uSupport_resolved', render: (t: Ticket) => formatDate(t.resolved) },
					]
				: []),
			{ key: 'LastUpdated', label: 'uSupport_lastUpdated', render: (t) => formatDate(t.lastUpdated) },
			{ key: 'LastUpdatedBy', label: 'uSupport_lastUpdatedBy', render: (t) => t.lastUpdatedBy },
		];
	}

	/** Reloads the current page; also called by parents after a ticket is created. */
	async load() {
		this._loading = true;
		const data = await this.#repository.getTickets(this.list, {
			page: this._page,
			searchTerm: this._search || undefined,
			sortColumn: this._sortColumn || undefined,
			sortDescending: this._sortDescending,
		});
		this._tickets = data?.items ?? [];
		this._totalPages = Math.max(1, data?.totalPages ?? 1);
		this._loading = false;
	}

	#onSearch(event: InputEvent) {
		this._search = (event.target as HTMLInputElement).value;
		this._page = 1;
		clearTimeout(this.#searchTimer);
		this.#searchTimer = window.setTimeout(() => this.load(), 300);
	}

	#sort(column: string) {
		this._sortDescending = this._sortColumn === column ? !this._sortDescending : true;
		this._sortColumn = column;
		this.load();
	}

	#onPageChange(event: Event) {
		this._page = (event.target as HTMLElement & { current: number }).current;
		this.load();
	}

	async #open(ticket: Ticket) {
		if (this._hasSection) {
			history.pushState(null, '', `/umbraco/${workspacePath(USUPPORT_TICKET_ENTITY_TYPE, `edit/${ticket.id}`)}`);
			return;
		}
		await umbOpenModal(this, USUPPORT_OPEN_TICKET_MODAL, { data: { id: ticket.id } }).catch(() => undefined);
		this.load();
	}

	render() {
		return html`
			<div id="toolbar">
				<uui-input
					label=${this.localize.term('placeholders_filter')}
					placeholder=${this.localize.term('placeholders_filter')}
					.value=${this._search}
					@input=${this.#onSearch}>
					<uui-icon name="icon-search" slot="prepend"></uui-icon>
				</uui-input>
			</div>
			${this._loading && !this._tickets.length ? html`<uui-loader-bar></uui-loader-bar>` : this.#renderTable()}
			${this._totalPages > 1
				? html`<uui-pagination .current=${this._page} .total=${this._totalPages} @change=${this.#onPageChange}></uui-pagination>`
				: nothing}
		`;
	}

	#renderTable() {
		if (!this._tickets.length) return html`<p>${this.localize.term('general_searchNoResult')}</p>`;

		const columns = this.#columns;
		return html`
			<uui-table>
				<uui-table-head>
					${columns.map(
						(column) => html`
							<uui-table-head-cell>
								<button class="sort" @click=${() => this.#sort(column.key)}>
									${this.localize.term(column.label)}
									${this._sortColumn === column.key
										? html`<uui-icon name=${this._sortDescending ? 'icon-navigation-down' : 'icon-navigation-up'}></uui-icon>`
										: nothing}
								</button>
							</uui-table-head-cell>
						`,
					)}
				</uui-table-head>
				${this._tickets.map(
					(ticket) => html`
						<uui-table-row selectable @click=${() => this.#open(ticket)}>
							${columns.map((column) => html`<uui-table-cell>${column.render(ticket)}</uui-table-cell>`)}
						</uui-table-row>
					`,
				)}
			</uui-table>
		`;
	}

	static styles = css`
		:host {
			display: block;
		}
		#toolbar {
			margin-bottom: var(--uui-size-space-4);
		}
		uui-table-row {
			cursor: pointer;
		}
		button.sort {
			all: unset;
			cursor: pointer;
			font-weight: bold;
		}
		uui-pagination {
			display: block;
			margin-top: var(--uui-size-space-4);
		}
	`;
}

declare global {
	interface HTMLElementTagNameMap {
		'usupport-tickets-table': USupportTicketsTableElement;
	}
}

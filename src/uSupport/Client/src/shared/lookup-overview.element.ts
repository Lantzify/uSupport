import { css, customElement, html } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { state } from '@umbraco-cms/backoffice/external/lit';
import { USupportRepository, type TicketStatus, type TicketType } from '../repository/usupport.repository.js';
import { workspacePath } from '../constants.js';
import { statusTag, typeIcon } from './display.js';
import { LOOKUPS, type LookupConfig } from './lookup.js';

interface Column<T> {
	label: string;
	render: (item: T) => unknown;
}

const check = (value: boolean) => html`<uui-icon name=${value ? 'icon-check' : 'icon-wrong'}></uui-icon>`;

abstract class LookupOverviewBase<T extends { id: string; name: string; alias: string }> extends UmbLitElement {
	protected abstract readonly config: LookupConfig;
	protected abstract readonly columns: Column<T>[];

	@state() private _items: T[] = [];

	async connectedCallback() {
		super.connectedCallback();
		this._items = ((await this.config.getAll(new USupportRepository(this))) ?? []) as unknown as T[];
	}

	#href = (path: string) => `/umbraco/${workspacePath(this.config.entityType, path)}`;

	render() {
		return html`
			<uui-button look="outline"
						 label=${this.localize.term('general_create')}
						 href=${this.#href('create')}
						 style="margin-bottom: var(--uui-size-space-6);"></uui-button>

			<uui-box class="overview">
				<uui-table>
					<uui-table-head>
						<uui-table-head-cell>${this.localize.term('general_name')}</uui-table-head-cell>
						<uui-table-head-cell>${this.localize.term('general_alias')}</uui-table-head-cell>
						${this.columns.map((column) => html`<uui-table-head-cell>${this.localize.term(column.label)}</uui-table-head-cell>`)}
					</uui-table-head>
					${this._items.map(
						(item) => html`
							<uui-table-row>
								<uui-table-cell><a href=${this.#href(`edit/${item.id}`)}>${item.name}</a></uui-table-cell>
								<uui-table-cell>${item.alias}</uui-table-cell>
								${this.columns.map((column) => html`<uui-table-cell>${column.render(item)}</uui-table-cell>`)}
							</uui-table-row>
						`,
					)}
				</uui-table>
			</uui-box>
		`;
	}

	static styles = css`
		:host {
			display: block;
			padding: var(--uui-size-layout-1);
		}
	`;
}

@customElement('usupport-ticket-types-overview')
export class USupportTicketTypesOverviewElement extends LookupOverviewBase<TicketType> {
	protected readonly config = LOOKUPS.type;
	protected readonly columns: Column<TicketType>[] = [
		{ label: 'uSupport_icon', render: (t) => typeIcon(t) },
		{ label: 'general_description', render: (t) => t.description },
		{ label: 'uSupport_usesProperty', render: (t) => check(!!t.dataTypeKey) },
	];
}

@customElement('usupport-ticket-statuses-overview')
export class USupportTicketStatusesOverviewElement extends LookupOverviewBase<TicketStatus> {
	protected readonly config = LOOKUPS.status;
	protected readonly columns: Column<TicketStatus>[] = [
		{ label: 'uSupport_preview', render: (s) => statusTag(s) },
		{ label: 'uSupport_default', render: (s) => check(s.default) },
		{ label: 'uSupport_active', render: (s) => check(s.active) },
	];
}

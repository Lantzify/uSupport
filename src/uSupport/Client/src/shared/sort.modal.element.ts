import { css, customElement, html, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbModalBaseElement } from '@umbraco-cms/backoffice/modal';
import { UmbSorterController } from '@umbraco-cms/backoffice/sorter';
import { USupportRepository } from '../repository/usupport.repository.js';
import { LOOKUPS, type LookupConfig } from './lookup.js';

interface SortItem {
	id: string;
	name: string;
	icon: string;
}

/** Drag-and-drop ordering of ticket types or statuses. */
@customElement('usupport-sort-modal')
export class USupportSortModalElement extends UmbModalBaseElement<{ lookup: LookupConfig['key'] }, string[]> {
	@state() private _items: SortItem[] = [];

	#sorter = new UmbSorterController<SortItem>(this, {
		getUniqueOfElement: (element) => element.dataset.id,
		getUniqueOfModel: (item) => item.id,
		identifier: 'uSupport.Sort',
		itemSelector: '.item',
		containerSelector: '#items',
		onChange: ({ model }) => (this._items = model),
	});

	async connectedCallback() {
		super.connectedCallback();
		const config = LOOKUPS[this.data!.lookup];
		const items = (await config.getAll(new USupportRepository(this))) ?? [];
		this._items = items.map((item) => ({ id: item.id, name: item.name, icon: config.itemIcon(item) }));
		this.#sorter.setModel(this._items);
	}

	#submit() {
		this.value = this._items.map((x) => x.id);
		this._submitModal();
	}

	render() {
		return html`
			<umb-body-layout headline=${this.localize.term('actions_sort')}>
				<uui-box>
					<p>${this.localize.term('sort_sortHelp')}</p>
					<div id="items">
						${repeat(
							this._items,
							(item) => item.id,
							(item) => html`<div class="item" data-id=${item.id}><umb-icon name=${item.icon}></umb-icon> ${item.name}</div>`,
						)}
					</div>
				</uui-box>
				<div slot="actions">
					<uui-button label=${this.localize.term('general_close')} @click=${this._rejectModal}></uui-button>
					<uui-button look="primary" color="positive" label=${this.localize.term('buttons_save')} @click=${this.#submit}></uui-button>
				</div>
			</umb-body-layout>
		`;
	}

	static styles = css`
		.item {
			display: flex;
			align-items: center;
			gap: var(--uui-size-space-3);
			padding: var(--uui-size-space-3);
			margin-bottom: var(--uui-size-space-2);
			border: 1px solid var(--uui-color-border);
			border-radius: var(--uui-border-radius);
			background: var(--uui-color-surface);
			cursor: grab;
		}
	`;
}

export default USupportSortModalElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-sort-modal': USupportSortModalElement;
	}
}

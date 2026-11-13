import { customElement, html, nothing, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbDataTypeDetailRepository } from '@umbraco-cms/backoffice/data-type';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import type { UmbPropertyDatasetElement } from '@umbraco-cms/backoffice/property';

const ALIAS = 'value';

const parse = (value?: string | null) => {
	if (!value) return undefined;
	try {
		return JSON.parse(value);
	} catch {
		// v13 values a v17 editor can't read are shown as stored
		return value;
	}
};

/**
 * Renders a ticket type's custom property with the data type the admin picked.
 * The value is kept as a JSON string, the same format v13 stored.
 */
@customElement('usupport-ticket-property')
export class USupportTicketPropertyElement extends UmbLitElement {
	@property({ attribute: false })
	dataTypeKey?: string | null;

	@property()
	label = '';

	@property()
	description = '';

	@property()
	value?: string | null;

	@property({ type: Boolean })
	readonly = false;

	@state() private _editorUiAlias?: string;
	@state() private _config: Array<{ alias: string; value: unknown }> = [];

	#repository = new UmbDataTypeDetailRepository(this);
	#loadedKey?: string | null;

	protected async willUpdate(changed: Map<string, unknown>) {
		if (!changed.has('dataTypeKey') || this.dataTypeKey === this.#loadedKey) return;
		this.#loadedKey = this.dataTypeKey;
		this._editorUiAlias = undefined;
		if (!this.dataTypeKey) return;

		const { data } = await this.#repository.requestByUnique(this.dataTypeKey);
		this._editorUiAlias = data?.editorUiAlias ?? undefined;
		this._config = data?.values ?? [];
	}

	#onChange(event: Event) {
		const dataset = event.target as UmbPropertyDatasetElement;
		const value = dataset.value.find((x) => x.alias === ALIAS)?.value;
		this.value = value === undefined ? null : JSON.stringify(value);
		this.dispatchEvent(new UmbChangeEvent());
	}

	render() {
		if (!this._editorUiAlias) return nothing;

		return html`
			<umb-property-dataset .value=${[{ alias: ALIAS, value: parse(this.value) }]} @change=${this.#onChange}>
				<umb-property
					alias=${ALIAS}
					label=${this.label}
					description=${this.description}
					property-editor-ui-alias=${this._editorUiAlias}
					.config=${this._config}
					?readonly=${this.readonly}></umb-property>
			</umb-property-dataset>
		`;
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'usupport-ticket-property': USupportTicketPropertyElement;
	}
}

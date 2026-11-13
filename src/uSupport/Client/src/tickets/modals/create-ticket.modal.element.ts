import { css, customElement, html, nothing, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbModalBaseElement } from '@umbraco-cms/backoffice/modal';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { USupportRepository, type TicketType } from '../../repository/usupport.repository.js';
import { USUPPORT_RICH_TEXT_DATA_TYPE_KEY, markupOf } from '../../constants.js';
import { typeIconName } from '../../shared/display.js';
import type { USupportTicketPropertyElement } from '../components/ticket-property.element.js';
import '../components/ticket-property.element.js';

/** Two steps: pick a ticket type, then fill in its custom property, title and summary. */
@customElement('usupport-create-ticket-modal')
export class USupportCreateTicketModalElement extends UmbModalBaseElement<never, { id: string }> {
	@state() private _types: TicketType[] = [];
	@state() private _type?: TicketType;
	@state() private _title = '';
	@state() private _summary: string | null = null;
	@state() private _propertyValue: string | null = null;
	@state() private _saving = false;

	#repository = new USupportRepository(this);

	async connectedCallback() {
		super.connectedCallback();
		this._types = (await this.#repository.getTypes()) ?? [];
	}

	async #submit() {
		if (!this._type || !this._title || !markupOf(this._summary)) return;

		this._saving = true;
		const ticket = await this.#repository.createTicket({
			typeId: this._type.id,
			title: this._title,
			summary: markupOf(this._summary),
			propertyValue: this._propertyValue,
		});
		this._saving = false;
		if (!ticket) return;

		(await this.getContext(UMB_NOTIFICATION_CONTEXT))?.peek('positive', {
			data: { message: this.localize.term('uSupport_ticketCreated', ticket.externalTicketId) },
		});
		this.value = { id: ticket.id };
		this._submitModal();
	}

	render() {
		return html`
			<uui-dialog-layout headline=${this.localize.term('uSupport_createTicket')}>
				<div id="content">
					${this._type ? this.#renderDetails(this._type) : this.#renderTypes()}
				</div>
				<div slot="actions">
					<uui-button label=${this.localize.term('general_close')} @click=${this._rejectModal}></uui-button>
					${this._type
						? html`
								<uui-button label=${this.localize.term('general_back')} @click=${() => (this._type = undefined)}></uui-button>
								<uui-button
									look="primary"
									color="positive"
									label=${this.localize.term('uSupport_createTicket')}
									.state=${this._saving ? 'waiting' : undefined}
									?disabled=${!this._title || !markupOf(this._summary)}
									@click=${this.#submit}></uui-button>
							`
						: nothing}
				</div>
			</uui-dialog-layout>
		`;
	}

	#renderTypes() {
		return html`
			<p class="subtitle">${this.localize.term('uSupport_chooseType')}</p>
			<div class="types">
				${this._types.map(
					(type) => html`
						<button class="type" type="button" @click=${() => (this._type = type)}>
							<umb-icon name=${typeIconName(type)}></umb-icon>
							<span>
								<strong>${type.name}</strong>
								<small>${type.description}</small>
							</span>
						</button>
					`,
				)}
			</div>
		`;
	}

	#renderDetails(type: TicketType) {
		return html`
			<umb-property-layout label=${this.localize.term('uSupport_title')} mandatory>
				<uui-input
					slot="editor"
					label=${this.localize.term('uSupport_title')}
					.value=${this._title}
					@input=${(e: InputEvent) => (this._title = (e.target as HTMLInputElement).value)}></uui-input>
			</umb-property-layout>
			<usupport-ticket-property
				.dataTypeKey=${USUPPORT_RICH_TEXT_DATA_TYPE_KEY}
				label=${this.localize.term('uSupport_summary')}
				.value=${this._summary}
				@change=${(e: Event) => (this._summary = (e.target as USupportTicketPropertyElement).value ?? null)}></usupport-ticket-property>
			<usupport-ticket-property
				.dataTypeKey=${type.dataTypeKey}
				label=${type.propertyName ?? ''}
				description=${type.propertyDescription ?? ''}
				.value=${this._propertyValue}
				@change=${(e: Event) => (this._propertyValue = (e.target as USupportTicketPropertyElement).value ?? null)}></usupport-ticket-property>
		`;
	}

	static styles = css`
		uui-dialog-layout {
			width: min(800px, 90vw);
		}
		#content {
			max-height: 70vh;
			overflow-y: auto;
		}
		.subtitle {
			margin-top: 0;
			color: var(--uui-color-text-alt);
		}
		.types {
			display: flex;
			flex-direction: column;
			gap: var(--uui-size-space-4);
		}
		.type {
			display: flex;
			align-items: center;
			gap: var(--uui-size-space-5);
			padding: var(--uui-size-space-5);
			border: 1px solid var(--uui-color-border);
			border-radius: var(--uui-border-radius);
			background: var(--uui-color-surface);
			color: var(--uui-color-text);
			font: inherit;
			text-align: left;
			cursor: pointer;
			transition: background-color 120ms;
		}
		.type:hover,
		.type:focus-visible {
			background: var(--uui-color-surface-emphasis);
		}
		.type umb-icon {
			font-size: var(--uui-size-14);
		}
		.type span {
			display: flex;
			flex-direction: column;
			gap: var(--uui-size-space-1);
		}
		.type small {
			color: var(--uui-color-text-alt);
		}
		uui-input {
			width: 100%;
		}
	`;
}

export default USupportCreateTicketModalElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-create-ticket-modal': USupportCreateTicketModalElement;
	}
}

import { customElement, html, state, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbModalBaseElement } from '@umbraco-cms/backoffice/modal';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { USupportRepository, type Ticket } from '../../repository/usupport.repository.js';
import { ticketDetails, ticketLayoutStyles } from '../components/ticket-details.js';
import '../components/comments.element.js';
import '../components/ticket-property.element.js';

/** Read-only ticket view for users without the uSupport section; they can still comment. */
@customElement('usupport-open-ticket-modal')
export class USupportOpenTicketModalElement extends UmbModalBaseElement<{ id: string }, never> {
	@state() private _ticket?: Ticket;

	#repository = new USupportRepository(this);

	async connectedCallback() {
		super.connectedCallback();
		if (this.data?.id) this._ticket = await this.#repository.getTicket(this.data.id);
	}

	render() {
		const ticket = this._ticket;
		return html`
			<umb-body-layout headline=${ticket?.externalTicketId ?? ''} main-no-padding>
				${ticket
					? html`
							<div class="layout">
								<div class="main">
									<uui-box>
										<umb-property-layout label='Ticket title'}>
											<p slot="editor">${ticket.title}</p>
										</umb-property-layout>
									
										<umb-property-layout label='Ticket summary'}>
											<p slot="editor">${unsafeHTML(ticket.summary)}</p>
										</umb-property-layout>
										
										${ticket.type?.dataTypeKey
											? html`<usupport-ticket-property
														readonly
														.dataTypeKey=${ticket.type.dataTypeKey}
														label=${ticket.type.propertyName ?? ''}
														.value=${ticket.propertyValue}></usupport-ticket-property>`
											: ''}
									</uui-box>
									<uui-box headline=${this.localize.term('uSupport_comments')}>
										<usupport-comments .ticketId=${ticket.id}></usupport-comments>
									</uui-box>
								</div>
								<div>${ticketDetails(ticket, this.localize)}</div>
							</div>
						`
					: html`<uui-loader-bar></uui-loader-bar>`}
				<uui-button slot="actions" label=${this.localize.term('general_close')} @click=${this._rejectModal}></uui-button>
			</umb-body-layout>
		`;
	}

	static styles = [UmbTextStyles, ticketLayoutStyles];
}

export default USupportOpenTicketModalElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-open-ticket-modal': USupportOpenTicketModalElement;
	}
}

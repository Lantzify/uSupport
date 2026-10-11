import { customElement, html, nothing, state, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { Ticket, TicketStatus, TicketType } from '../../../repository/usupport.repository.js';
import { USUPPORT_TICKET_WORKSPACE_CONTEXT, type TicketDraft, type USupportTicketWorkspaceContext } from '../ticket-workspace.context.js';
import { ticketDetails, ticketLayoutStyles } from '../../ticket-details.js';
import '../../comments.element.js';
import '../../ticket-property.element.js';

@customElement('usupport-ticket-view')
export class USupportTicketViewElement extends UmbLitElement {
	@state() private _ticket?: Ticket;
	@state() private _draft?: TicketDraft;
	@state() private _types: TicketType[] = [];
	@state() private _statuses: TicketStatus[] = [];

	#context?: USupportTicketWorkspaceContext;

	constructor() {
		super();
		this.consumeContext(USUPPORT_TICKET_WORKSPACE_CONTEXT, (context) => {
			this.#context = context;
			this.observe(context?.ticket, (ticket) => (this._ticket = ticket));
			this.observe(context?.draft, (draft) => (this._draft = draft));
			this.observe(context?.types, (types) => (this._types = types ?? []));
			this.observe(context?.statuses, (statuses) => (this._statuses = statuses ?? []));
		});
	}

	#options = (items: Array<{ id: string; name: string }>, selected?: string) =>
		items.map((item) => ({ name: item.name, value: item.id, selected: item.id === selected }));

	render() {
		const ticket = this._ticket;
		const draft = this._draft;
		if (!ticket || !draft) return nothing;

		// Changing the type clears the custom property on save, so only show it while the type is unchanged.
		const showProperty = ticket.type?.dataTypeKey && draft.typeId === ticket.typeId;

		return html`
			<div class="layout">
				<div class="main">
					
					<uui-box headline=${this.localize.term('uSupport_ticket')}>
						<umb-property-layout label='Ticket title'}>
							<p slot="editor">${ticket.title}</p>
						</umb-property-layout>

						<umb-property-layout label='Ticket summary'}>
							<p slot="editor">${unsafeHTML(ticket.summary)}</p>
						</umb-property-layout>

						<hr />
						
						${showProperty
							? html`<usupport-ticket-property
									readonly
									.dataTypeKey=${ticket.type!.dataTypeKey}
									label=${ticket.type!.propertyName ?? ''}
									description=${ticket.type!.propertyDescription ?? ''}
									.value=${ticket.propertyValue}></usupport-ticket-property>`
							: nothing}
						<umb-property-layout label=${this.localize.term('uSupport_type')}>
							<uui-select
								slot="editor"
								label=${this.localize.term('uSupport_type')}
								.options=${this.#options(this._types, draft.typeId)}
								@change=${(e: Event) => this.#context?.updateDraft({ typeId: (e.target as HTMLSelectElement).value })}></uui-select>
						</umb-property-layout>
						<umb-property-layout label=${this.localize.term('uSupport_status')}>
							<uui-select
								slot="editor"
								label=${this.localize.term('uSupport_status')}
								.options=${this.#options(this._statuses, draft.statusId)}
								@change=${(e: Event) => this.#context?.updateDraft({ statusId: (e.target as HTMLSelectElement).value })}></uui-select>
						</umb-property-layout>
						<umb-property-layout
							label=${this.localize.term('uSupport_internalComment')}
							description=${this.localize.term('uSupport_internalCommentDescription')}>
							<uui-textarea
								slot="editor"
								label=${this.localize.term('uSupport_internalComment')}
								.value=${draft.internalComment ?? ''}
								@input=${(e: InputEvent) =>
									this.#context?.updateDraft({ internalComment: (e.target as HTMLTextAreaElement).value })}></uui-textarea>
						</umb-property-layout>
					</uui-box>
					<uui-box headline=${this.localize.term('uSupport_comments')}>
						<usupport-comments .ticketId=${ticket.id}></usupport-comments>
					</uui-box>
				</div>
				<div>${ticketDetails(ticket, this.localize)}</div>
			</div>
		`;
	}

	static styles = [UmbTextStyles, ticketLayoutStyles];
}

export default USupportTicketViewElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-ticket-view': USupportTicketViewElement;
	}
}

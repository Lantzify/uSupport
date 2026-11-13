import { css, customElement, html, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { USUPPORT_TICKETS_ROOT_ENTITY_TYPE, workspacePath } from '../../constants.js';
import type { Ticket } from '../../repository/usupport.repository.js';
import { USUPPORT_TICKET_WORKSPACE_CONTEXT, type USupportTicketWorkspaceContext } from './ticket-workspace.context.js';

const OVERVIEW_PATH = `/umbraco/${workspacePath(USUPPORT_TICKETS_ROOT_ENTITY_TYPE)}`;

/** Ticket workspace shell; the Ticket and History tabs are workspaceView extensions. */
@customElement('usupport-ticket-editor')
export class USupportTicketEditorElement extends UmbLitElement {
	@state() private _ticket?: Ticket;
	@state() private _saving?: 'save' | 'email';

	#context?: USupportTicketWorkspaceContext;

	constructor() {
		super();
		this.consumeContext(USUPPORT_TICKET_WORKSPACE_CONTEXT, (context) => {
			this.#context = context;
			this.observe(context?.ticket, (ticket) => (this._ticket = ticket));
		});
	}

	async #save(sendEmail: boolean) {
		this._saving = sendEmail ? 'email' : 'save';
		await this.#context?.save(sendEmail);
		this._saving = undefined;
	}

	render() {
		return html`
			<umb-workspace-editor
				back-path=${OVERVIEW_PATH}
				?loading=${!this._ticket}>
				<h3 slot="header">${this._ticket ? `${this._ticket.externalTicketId} – ${this._ticket.title}` : ''}</h3>
				<umb-workspace-entity-action-menu slot="action-menu"></umb-workspace-entity-action-menu>
				<uui-button
					slot="actions"
					look="secondary"
					color="positive"
					label=${this.localize.term('buttons_save')}
					.state=${this._saving === 'save' ? 'waiting' : undefined}
					@click=${() => this.#save(false)}></uui-button>
				<uui-button
					slot="actions"
					look="primary"
					color="positive"
					label=${this.localize.term('uSupport_saveAndSendEmail')}
					.state=${this._saving === 'email' ? 'waiting' : undefined}
					@click=${() => this.#save(true)}></uui-button>

			</umb-workspace-editor>
		`;
	}

	static styles = css`
		h3 {
			margin: 0;
			align-self: center;
		}
	`;
}

export default USupportTicketEditorElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-ticket-editor': USupportTicketEditorElement;
	}
}

import { css, customElement, html, query } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { USUPPORT_CREATE_TICKET_MODAL } from '../tickets/modals/tokens.js';
import type { USupportTicketsTableElement } from '../tickets/components/tickets-table.element.js';
import '../tickets/components/tickets-table.element.js';

/** "Support" dashboard in the Content section: create tickets and follow your tickets. */
@customElement('usupport-user-dashboard')
export class USupportUserDashboardElement extends UmbLitElement {
	@query('usupport-tickets-table[list="active"]')
	private _activeTable?: USupportTicketsTableElement;

	async #createTicket() {
		const result = await umbOpenModal(this, USUPPORT_CREATE_TICKET_MODAL).catch(() => undefined);
		if (result) this._activeTable?.load();
	}

	render() {
		return html`
			<div class="usupport-dashboard">
				<uui-button
						style="margin-bottom: var(--uui-size-layout-1);"
						look="outline"
						color="default"
						label=${this.localize.term('uSupport_createTicket')}
						@click=${this.#createTicket}></uui-button>

				<div class="main">
					<uui-box headline=${this.localize.term('uSupport_activeTickets')}>
						<usupport-tickets-table list="active"></usupport-tickets-table>
					</uui-box>
					<uui-box headline=${this.localize.term('uSupport_resolvedTickets')}>
						<usupport-tickets-table list="resolved"></usupport-tickets-table>
					</uui-box>
				</div>
			</div>
		`;
	}

	static styles = css`
		.usupport-dashboard {
			padding: var(--uui-size-layout-1);
		}
		.main {
			display: flex;
			flex-direction: column;
			gap: var(--uui-size-layout-1);
		}
	`;
}

export default USupportUserDashboardElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-user-dashboard': USupportUserDashboardElement;
	}
}

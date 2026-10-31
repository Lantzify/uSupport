import { css, customElement, html } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import '../components/tickets-table.element.js';

/** Tickets overview in the uSupport section: active and resolved tickets. */
@customElement('usupport-tickets-overview-view')
export class USupportTicketsOverviewViewElement extends UmbLitElement {
	render() {
		return html`
			<uui-box headline=${this.localize.term('uSupport_activeTickets')}>
				<usupport-tickets-table list="active"></usupport-tickets-table>
			</uui-box>
			<uui-box headline=${this.localize.term('uSupport_resolvedTickets')}>
				<usupport-tickets-table list="resolved"></usupport-tickets-table>
			</uui-box>
		`;
	}

	static styles = css`
		:host {
			display: flex;
			flex-direction: column;
			gap: var(--uui-size-layout-1);
			padding: var(--uui-size-layout-1);
		}
	`;
}

export default USupportTicketsOverviewViewElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-tickets-overview-view': USupportTicketsOverviewViewElement;
	}
}

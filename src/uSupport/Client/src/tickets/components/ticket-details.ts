import { css, html, nothing } from '@umbraco-cms/backoffice/external/lit';
import type { UmbLocalizationController } from '@umbraco-cms/backoffice/localization-api';
import type { Ticket } from '../../repository/usupport.repository.js';
import { formatDate, statusTag, typeIconName } from '../../shared/display.js';

export const ticketDetails = (ticket: Ticket, localize: UmbLocalizationController) => {
	const row = (label: string, value: unknown) =>
		value ? html`<div class="detail"><strong>${localize.term(label)}</strong><span>${value}</span></div>` : nothing;

	return html`
		<uui-box headline=${localize.term('uSupport_details')}>
			${row('uSupport_ticketId', ticket.externalTicketId)}
			${row(
				'uSupport_type',
				ticket.type &&
					html`<uui-ref-node readonly name=${ticket.type.name} detail=${ticket.type.description ?? ''}>
						<umb-icon slot="icon" name=${typeIconName(ticket.type)}></umb-icon>
					</uui-ref-node>`,
			)}
			${row('uSupport_status', statusTag(ticket.status))}
			${row('uSupport_author', ticket.author?.name)}
			${row('uSupport_submitted', formatDate(ticket.submitted))}
			${row('uSupport_lastUpdated', formatDate(ticket.lastUpdated))}
			${row('uSupport_lastUpdatedBy', ticket.lastUpdatedBy)}
			${row('uSupport_resolved', formatDate(ticket.resolved))}
		</uui-box>
	`;
};

export const ticketLayoutStyles = css`
	.layout {
		display: grid;
		grid-template-columns: 1fr 320px;
		gap: var(--uui-size-layout-1);
		padding: var(--uui-size-layout-1);
	}
	.main {
		display: flex;
		flex-direction: column;
		gap: var(--uui-size-layout-1);
	}
	.detail {
		display: flex;
		flex-direction: column;
		margin-bottom: var(--uui-size-space-4);
	}
	.detail uui-ref-node {
		margin: calc(-1 * var(--uui-size-3));
		margin-top: calc(-1 * var(--uui-size-2));
		background: none;
	}
	@media (max-width: 1000px) {
		.layout {
			grid-template-columns: 1fr;
		}
	}
`;

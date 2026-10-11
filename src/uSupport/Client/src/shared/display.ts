import { html, nothing } from '@umbraco-cms/backoffice/external/lit';
import type { TicketStatus } from '../repository/usupport.repository.js';

/** Status colours are stored as v13 badge names; map them onto uui-tag. */
const STATUS_TAG: Record<string, { color: string; look: string }> = {
	primary: { color: 'default', look: 'primary' },
	secondary: { color: 'default', look: 'secondary' },
	success: { color: 'positive', look: 'primary' },
	warning: { color: 'warning', look: 'primary' },
	danger: { color: 'danger', look: 'primary' },
	gray: { color: 'default', look: 'outline' },
};
export const STATUS_COLORS = Object.keys(STATUS_TAG);

export const statusTag = (status?: Pick<TicketStatus, 'name' | 'color'> | null) => {
	if (!status) return nothing;
	const tag = STATUS_TAG[status.color ?? ''] ?? STATUS_TAG.secondary;
	return html`<uui-tag color=${tag.color} look=${tag.look}>${status.name}</uui-tag>`;
};

/** Types store icon and colour separately (e.g. "icon-layout" + "color-indigo"). */
export const typeIconName = (type?: { icon?: string | null; color?: string | null } | null) =>
	[type?.icon || 'icon-ticket', type?.color].filter(Boolean).join(' ');

export const typeIcon = (type?: { name?: string | null; icon?: string | null; color?: string | null } | null) =>
	html`<umb-icon name=${typeIconName(type)} title=${type?.name ?? ''} style="font-size: var(--uui-size-8); display:block;"></umb-icon>`;

export const formatDate = (value?: string | null) => (value ? new Date(value).toLocaleString() : '');

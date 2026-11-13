import { html, nothing } from '@umbraco-cms/backoffice/external/lit';
import type { TicketStatus } from '../repository/usupport.repository.js';

/** Status colours are stored as v13 badge names; map them onto the UUI palette tokens closest to the v13 badges (background, text). */
const STATUS_TAG: Record<string, [string, string]> = {
	primary: ['--uui-palette-violet-blue', '--uui-palette-white'],
	secondary: ['--uui-palette-space-cadet', '--uui-palette-white'],
	success: ['--uui-palette-jungle-green', '--uui-palette-white'],
	warning: ['--uui-palette-deep-saffron', '--uui-palette-white'],
	danger: ['--uui-palette-maroon-flush', '--uui-palette-white'],
	gray: ['--uui-palette-timberwolf', '--uui-palette-violet-blue'],
};
export const STATUS_COLORS = Object.keys(STATUS_TAG);

export const statusTag = (status?: Pick<TicketStatus, 'name' | 'color'> | null) => {
	if (!status) return nothing;
	const [background, text] = STATUS_TAG[status.color ?? ''] ?? STATUS_TAG.secondary;
	return html`<uui-tag style="background-color: var(${background}); border-color: var(${background}); color: var(${text})"
		>${status.name}</uui-tag
	>`;
};

/** Types store icon and colour separately (e.g. "icon-layout" + "color-indigo"). */
export const typeIconName = (type?: { icon?: string | null; color?: string | null } | null) =>
	[type?.icon || 'icon-ticket', type?.color].filter(Boolean).join(' ');

export const typeIcon = (type?: { name?: string | null; icon?: string | null; color?: string | null } | null) =>
	html`<umb-icon name=${typeIconName(type)} title=${type?.name ?? ''} style="font-size: var(--uui-size-8); display:block;"></umb-icon>`;

export const formatDate = (value?: string | null) => (value ? new Date(value).toLocaleString() : '');

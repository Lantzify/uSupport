import { UmbModalToken } from '@umbraco-cms/backoffice/modal';

export const USUPPORT_CREATE_TICKET_MODAL = new UmbModalToken<never, { id: string }>('uSupport.Modal.CreateTicket', {
	modal: { type: 'dialog' },
});

export const USUPPORT_OPEN_TICKET_MODAL = new UmbModalToken<{ id: string }, never>('uSupport.Modal.OpenTicket', {
	modal: { type: 'sidebar', size: 'large' },
});

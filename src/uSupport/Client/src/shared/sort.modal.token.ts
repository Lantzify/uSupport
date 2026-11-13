import { UmbModalToken } from '@umbraco-cms/backoffice/modal';
import type { LookupConfig } from './lookup.js';

/** Returns the item ids in their new order. */
export const USUPPORT_SORT_MODAL = new UmbModalToken<{ lookup: LookupConfig['key'] }, string[]>('uSupport.Modal.Sort', {
	modal: { type: 'sidebar', size: 'small' },
});

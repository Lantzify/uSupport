import { LOOKUPS, createLookupTreeRepository } from '../shared/lookup.js';

export const api = createLookupTreeRepository(LOOKUPS.type, 'icon-ticket');

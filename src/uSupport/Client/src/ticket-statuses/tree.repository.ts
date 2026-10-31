import { LOOKUPS, createLookupTreeRepository } from '../shared/lookup.js';

export const api = createLookupTreeRepository(LOOKUPS.status, 'icon-file-cabinet');

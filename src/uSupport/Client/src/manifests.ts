import { manifests as section } from './section/manifest.js';
import { manifests as dashboards } from './dashboards/manifest.js';
import { manifests as tickets } from './tickets/manifest.js';
import { manifests as ticketTypes } from './ticket-types/manifest.js';
import { manifests as ticketStatuses } from './ticket-statuses/manifest.js';
import { manifests as settings } from './settings/manifest.js';
import { manifests as localization } from './localization/manifest.js';

const shared: Array<UmbExtensionManifest> = [
	{
		type: 'modal',
		alias: 'uSupport.Modal.Sort',
		name: 'uSupport Sort Modal',
		element: () => import('./shared/sort.modal.element.js'),
	},
];

export const manifests: Array<UmbExtensionManifest> = [
	...shared,
	...section,
	...dashboards,
	...tickets,
	...ticketTypes,
	...ticketStatuses,
	...settings,
	...localization,
];

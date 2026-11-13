import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbTreeRepositoryBase, UmbTreeServerDataSourceBase, type UmbTreeItemModel, type UmbTreeRootModel } from '@umbraco-cms/backoffice/tree';
import { USupportRepository, type DependentTicket } from '../repository/usupport.repository.js';
import {
	USUPPORT_TICKET_STATUS_ENTITY_TYPE,
	USUPPORT_TICKET_STATUS_ROOT_ENTITY_TYPE,
	USUPPORT_TICKET_TYPE_ENTITY_TYPE,
	USUPPORT_TICKET_TYPE_ROOT_ENTITY_TYPE,
} from '../constants.js';
import { typeIconName } from './display.js';

interface LookupItem {
	id: string;
	name: string;
	icon?: string | null;
	color?: string | null;
}

/**
 * Ticket types and ticket statuses behave the same in the backoffice (tree, create, sort, delete);
 * this describes the differences.
 */
export interface LookupConfig {
	key: 'type' | 'status';
	label: string;
	rootEntityType: string;
	entityType: string;
	itemIcon: (item: LookupItem) => string;
	getAll: (repository: USupportRepository) => Promise<LookupItem[] | undefined>;
	getDependents: (repository: USupportRepository, id: string) => Promise<DependentTicket[] | undefined>;
	delete: (repository: USupportRepository, id: string) => Promise<boolean>;
	sort: (repository: USupportRepository, ids: string[]) => Promise<unknown>;
}

export const LOOKUPS: Record<LookupConfig['key'], LookupConfig> = {
	type: {
		key: 'type',
		label: '#uSupport_ticketTypes',
		rootEntityType: USUPPORT_TICKET_TYPE_ROOT_ENTITY_TYPE,
		entityType: USUPPORT_TICKET_TYPE_ENTITY_TYPE,
		itemIcon: (item) => typeIconName(item),
		getAll: (r) => r.getTypes(),
		getDependents: (r, id) => r.getTypeDependents(id),
		delete: (r, id) => r.deleteType(id),
		sort: (r, ids) => r.sortTypes(ids),
	},
	status: {
		key: 'status',
		label: '#uSupport_ticketStatuses',
		rootEntityType: USUPPORT_TICKET_STATUS_ROOT_ENTITY_TYPE,
		entityType: USUPPORT_TICKET_STATUS_ENTITY_TYPE,
		itemIcon: () => 'icon-tag',
		getAll: (r) => r.getStatuses(),
		getDependents: (r, id) => r.getStatusDependents(id),
		delete: (r, id) => r.deleteStatus(id),
		sort: (r, ids) => r.sortStatuses(ids),
	},
};

/** Flat tree (root + one level) listing every item of a lookup. */
export const createLookupTreeRepository = (config: LookupConfig, rootIcon: string) => {
	class DataSource extends UmbTreeServerDataSourceBase<LookupItem & { hasChildren: boolean }, UmbTreeItemModel> {
		constructor(host: UmbControllerHost) {
			const repository = new USupportRepository(host);
			super(host, {
				getRootItems: async () => {
					const items = ((await config.getAll(repository)) ?? []).map((item) => ({ ...item, hasChildren: false }));
					return { data: { items, total: items.length } };
				},
				getChildrenOf: async () => ({ data: { items: [], total: 0 } }),
				getAncestorsOf: async () => ({ data: [] }),
				mapper: (item) => ({
					unique: item.id,
					parent: { unique: null, entityType: config.rootEntityType },
					entityType: config.entityType,
					name: item.name,
					icon: config.itemIcon(item),
					hasChildren: false,
					isFolder: false,
				}),
			});
		}
	}

	return class extends UmbTreeRepositoryBase<UmbTreeItemModel, UmbTreeRootModel> {
		constructor(host: UmbControllerHost) {
			super(host, DataSource);
		}

		async requestTreeRoot() {
			const data: UmbTreeRootModel = {
				unique: null,
				entityType: config.rootEntityType,
				name: config.label,
				icon: rootIcon,
				hasChildren: true,
				isFolder: true,
			};
			return { data };
		}
	};
};

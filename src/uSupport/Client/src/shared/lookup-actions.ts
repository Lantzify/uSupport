import { html } from '@umbraco-cms/backoffice/external/lit';
import { UmbEntityActionBase, UmbRequestReloadChildrenOfEntityEvent } from '@umbraco-cms/backoffice/entity-action';
import { UMB_ACTION_EVENT_CONTEXT } from '@umbraco-cms/backoffice/action';
import { umbConfirmModal, umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { UmbLocalizationController } from '@umbraco-cms/backoffice/localization-api';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { USupportRepository } from '../repository/usupport.repository.js';
import { USUPPORT_TICKET_ENTITY_TYPE, workspacePath } from '../constants.js';
import { LOOKUPS, type LookupConfig } from './lookup.js';
import { USUPPORT_SORT_MODAL } from './sort.modal.token.js';

export interface LookupActionMeta {
	lookup: LookupConfig['key'];
}

abstract class LookupActionBase extends UmbEntityActionBase<LookupActionMeta> {
	protected get config() {
		return LOOKUPS[this.args.meta.lookup];
	}

	protected repository = new USupportRepository(this);
	protected localize = new UmbLocalizationController(this);

	protected async reloadTree() {
		const events = await this.getContext(UMB_ACTION_EVENT_CONTEXT);
		events?.dispatchEvent(new UmbRequestReloadChildrenOfEntityEvent({ entityType: this.config.rootEntityType, unique: null }));
	}
}

/** "Create" on the tree root: opens the workspace in create mode. */
export class CreateLookupAction extends LookupActionBase {
	async getHref() {
		return `/umbraco/${workspacePath(this.config.entityType, 'create')}`;
	}
}

/** "Sort" on the tree root. */
export class SortLookupAction extends LookupActionBase {
	async execute() {
		const ids = await umbOpenModal(this, USUPPORT_SORT_MODAL, { data: { lookup: this.args.meta.lookup } }).catch(() => undefined);
		if (!ids) return;
		await this.config.sort(this.repository, ids);
		await this.reloadTree();
	}
}

/** "Delete" on an item; refused while tickets still use it. */
export class DeleteLookupAction extends LookupActionBase {
	async execute() {
		const id = this.args.unique;
		if (!id) return;

		const dependents = (await this.config.getDependents(this.repository, id)) ?? [];
		if (dependents.length) {
			await umbConfirmModal(this, {
				headline: this.localize.term('uSupport_cannotDelete'),
				content: html`
					<p>${this.localize.term('uSupport_usedByTickets')}</p>
					<ul>
						${dependents.map(
							(ticket) =>
								html`<li><a href=${`/umbraco/${workspacePath(USUPPORT_TICKET_ENTITY_TYPE, `edit/${ticket.id}`)}`}>${ticket.name}</a></li>`,
						)}
					</ul>
				`,
				confirmLabel: this.localize.term('general_ok'),
			}).catch(() => undefined);
			return;
		}

		await umbConfirmModal(this, {
			headline: this.localize.term('actions_delete'),
			content: this.localize.term('defaultdialogs_confirmdelete') + '?',
			color: 'danger',
			confirmLabel: this.localize.term('actions_delete'),
		});
		if (!(await this.config.delete(this.repository, id))) return;
		await this.reloadTree();
		(await this.getContext(UMB_NOTIFICATION_CONTEXT))?.peek('positive', { data: { message: this.localize.term('general_deleted') } });

		// Deleted from its own workspace ("..." menu): leave the now-missing item.
		if (location.pathname.endsWith(`/edit/${id}`)) history.pushState(null, '', `/umbraco/${workspacePath(this.config.rootEntityType)}`);
	}
}

import { UmbEntityActionBase } from '@umbraco-cms/backoffice/entity-action';
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UmbLocalizationController } from '@umbraco-cms/backoffice/localization-api';
import { USupportRepository } from '../repository/usupport.repository.js';
import { USUPPORT_TICKETS_ROOT_ENTITY_TYPE, workspacePath } from '../constants.js';

/** "Delete" in the ticket workspace's "..." menu. */
export class USupportDeleteTicketAction extends UmbEntityActionBase<never> {
	#repository = new USupportRepository(this);
	#localize = new UmbLocalizationController(this);

	async execute() {
		const id = this.args.unique;
		if (!id) return;

		await umbConfirmModal(this, {
			headline: this.#localize.term('actions_delete'),
			content: this.#localize.term('defaultdialogs_confirmdelete') + '?',
			color: 'danger',
			confirmLabel: this.#localize.term('actions_delete'),
		});
		await this.#repository.deleteTicket(id);
		history.pushState(null, '', `/umbraco/${workspacePath(USUPPORT_TICKETS_ROOT_ENTITY_TYPE)}`);
	}
}

export { USupportDeleteTicketAction as api };

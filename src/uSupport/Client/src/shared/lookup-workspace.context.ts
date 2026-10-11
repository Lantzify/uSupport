import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import { UmbObjectState } from '@umbraco-cms/backoffice/observable-api';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { UmbLocalizationController } from '@umbraco-cms/backoffice/localization-api';
import { UMB_ACTION_EVENT_CONTEXT } from '@umbraco-cms/backoffice/action';
import { UmbEntityContext } from '@umbraco-cms/backoffice/entity';
import { UmbRequestReloadChildrenOfEntityEvent } from '@umbraco-cms/backoffice/entity-action';
import {
	UMB_WORKSPACE_CONTEXT,
	UmbWorkspaceRouteManager,
	type UmbRoutableWorkspaceContext,
	type UmbWorkspaceContext,
} from '@umbraco-cms/backoffice/workspace';
import { USupportRepository } from '../repository/usupport.repository.js';
import { workspacePath } from '../constants.js';
import type { LookupConfig } from './lookup.js';

export interface LookupModel {
	id: string;
	name: string;
	alias: string;
}

/** Create/edit workspace shared by ticket types and ticket statuses. */
export abstract class USupportLookupWorkspaceContext<T extends LookupModel = LookupModel>
	extends UmbContextBase
	implements UmbRoutableWorkspaceContext
{
	abstract readonly workspaceAlias: string;
	protected abstract readonly config: LookupConfig;
	protected abstract readonly editor: () => Promise<{ default: CustomElementConstructor }>;
	protected abstract readonly empty: () => T;
	protected abstract readonly get: (id: string) => Promise<T | undefined>;
	protected abstract readonly create: (model: T) => Promise<T | undefined>;
	protected abstract readonly update: (model: T) => Promise<T | undefined>;

	readonly routes = new UmbWorkspaceRouteManager(this);
	protected readonly repository = new USupportRepository(this);
	#localize = new UmbLocalizationController(this);

	#model = new UmbObjectState<T | undefined>(undefined);
	readonly model = this.#model.asObservable();

	/** Entity type + unique for Umbraco's workspace entity action menu ("..."). */
	#entityContext = new UmbEntityContext(this);
	readonly unique = this.#entityContext.unique;

	#isNew = new UmbObjectState(false);
	readonly isNew = this.#isNew.asObservable();

	constructor(host: UmbControllerHost) {
		super(host, UMB_WORKSPACE_CONTEXT);
		this.observe(this.model, (model) => this.#entityContext.setUnique(model?.id ?? null));
		// Defer until subclass fields (editor, config) are initialised.
		queueMicrotask(() => {
			this.#entityContext.setEntityType(this.config.entityType);
			this.routes.setRoutes([
				{
					path: 'create',
					component: this.editor,
					setup: () => {
						this.#isNew.setValue(true);
						this.#model.setValue(this.empty());
					},
				},
				{
					path: 'edit/:id',
					component: this.editor,
					setup: async (_component, info) => {
						this.#isNew.setValue(false);
						this.#model.setValue(await this.get(info.match.params.id));
					},
				},
			]);
		});
	}

	getEntityType = () => this.config.entityType;
	getUnique = () => this.#model.getValue()?.id;
	getConfig = () => this.config;

	updateModel(partial: Partial<T>) {
		this.#model.update(partial);
	}

	async save() {
		const model = this.#model.getValue();
		if (!model) return;

		const isNew = this.#isNew.getValue();
		const saved = isNew ? await this.create(model) : await this.update(model);
		if (!saved) return;

		this.#model.setValue(saved);
		this.#isNew.setValue(false);

		(await this.getContext(UMB_NOTIFICATION_CONTEXT))?.peek('positive', {
			data: { message: this.#localize.term('speechBubbles_editContentSavedHeader') },
		});
		(await this.getContext(UMB_ACTION_EVENT_CONTEXT))?.dispatchEvent(
			new UmbRequestReloadChildrenOfEntityEvent({ entityType: this.config.rootEntityType, unique: null }),
		);

		if (isNew) history.replaceState(null, '', `/umbraco/${workspacePath(this.config.entityType, `edit/${saved.id}`)}`);
	}
}

export const USUPPORT_LOOKUP_WORKSPACE_CONTEXT = new UmbContextToken<UmbWorkspaceContext, USupportLookupWorkspaceContext>(
	'UmbWorkspaceContext',
	undefined,
	(context): context is USupportLookupWorkspaceContext => context instanceof USupportLookupWorkspaceContext,
);

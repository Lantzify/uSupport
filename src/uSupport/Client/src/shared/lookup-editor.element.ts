import { css, customElement, html, nothing, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { UMB_ICON_PICKER_MODAL } from '@umbraco-cms/backoffice/icon';
import { typeIcon } from './display.js';
import { workspacePath } from '../constants.js';
import { USUPPORT_LOOKUP_WORKSPACE_CONTEXT, type LookupModel, type USupportLookupWorkspaceContext } from './lookup-workspace.context.js';

type IconLookupModel = LookupModel & { icon?: string | null; color?: string | null; description?: string | null };

/**
 * Workspace chrome for a ticket type/status, built from the same parts as Umbraco's document type header
 * (umb-input-with-alias, entity action menu). `with-icon` adds the icon picker and description field.
 */
@customElement('usupport-lookup-editor')
export class USupportLookupEditorElement extends UmbLitElement {
	@property({ type: Boolean, attribute: 'with-icon' }) withIcon = false;

	@state() private _model?: IconLookupModel;
	@state() private _isNew = false;
	@state() private _saving = false;

	#context?: USupportLookupWorkspaceContext<IconLookupModel>;

	constructor() {
		super();
		this.consumeContext(USUPPORT_LOOKUP_WORKSPACE_CONTEXT, (context) => {
			this.#context = context as USupportLookupWorkspaceContext<IconLookupModel> | undefined;
			this.observe(context?.model, (model) => (this._model = model));
			this.observe(context?.isNew, (isNew) => (this._isNew = isNew ?? false));
		});
	}

	async #pickIcon() {
		const result = await umbOpenModal(this, UMB_ICON_PICKER_MODAL, {
			value: { icon: this._model?.icon ?? undefined, color: this._model?.color?.replace(/^color-/, '') },
		}).catch(() => undefined);
		if (!result) return;
		this.#context?.updateModel({ icon: result.icon ?? 'icon-ticket', color: result.color ? `color-${result.color}` : null });
	}

	async #save() {
		this._saving = true;
		await this.#context?.save();
		this._saving = false;
	}

	render() {
		const config = this.#context?.getConfig();
		return html`
			<umb-workspace-editor back-path=${config ? `/umbraco/${workspacePath(config.rootEntityType)}` : ''} ?loading=${!this._model}>
				<div id="header" slot="header">
					${this.withIcon
						? html`<uui-button id="icon" compact look="outline" label=${this.localize.term('uSupport_icon')} @click=${this.#pickIcon}>
								${typeIcon(this._model)}
							</uui-button>`
						: nothing}
					<div id="editors">
						<umb-input-with-alias
							id="name"
							label=${this.localize.term('placeholders_entername')}
							.value=${this._model?.name ?? ''}
							.alias=${this._model?.alias ?? ''}
							?auto-generate-alias=${this._isNew}
							required
							@change=${(e: Event) => {
								const input = e.target as HTMLInputElement & { alias?: string };
								this.#context?.updateModel({ name: input.value ?? '', alias: input.alias ?? '' });
							}}></umb-input-with-alias>
						${this.withIcon
							? html`<uui-input
									id="description"
									label=${this.localize.term('placeholders_enterDescription')}
									placeholder=${this.localize.term('placeholders_enterDescription')}
									.value=${this._model?.description ?? ''}
									@input=${(e: InputEvent) =>
										this.#context?.updateModel({ description: (e.target as HTMLInputElement).value })}></uui-input>`
							: nothing}
					</div>
				</div>
				${this._isNew ? nothing : html`<umb-workspace-entity-action-menu slot="action-menu"></umb-workspace-entity-action-menu>`}
				<div id="main"><slot></slot></div>
				<uui-button
					slot="actions"
					look="primary"
					color="positive"
					label=${this.localize.term('buttons_save')}
					.state=${this._saving ? 'waiting' : undefined}
					?disabled=${!this._model?.name}
					@click=${this.#save}></uui-button>
			</umb-workspace-editor>
		`;
	}

	static styles = css`
		#header {
			display: flex;
			flex: 1;
			gap: var(--uui-size-space-2);
		}
		#editors {
			display: flex;
			flex: 1;
			flex-direction: column;
			gap: 1px;
		}
		#name {
			width: 100%;
			z-index: 1;
		}
		#description {
			width: 100%;
			--uui-input-height: var(--uui-size-8);
			--uui-input-border-color: transparent;
		}
		#description:hover {
			--uui-input-border-color: var(--uui-color-border);
		}
		#icon {
			font-size: var(--uui-size-8);
			height: 60px;
			width: 60px;
			--uui-button-border-color: transparent;
			--uui-button-border-color-hover: var(--uui-color-border);
		}
		#main {
			padding: var(--uui-size-layout-1);
		}
	`;
}

declare global {
	interface HTMLElementTagNameMap {
		'usupport-lookup-editor': USupportLookupEditorElement;
	}
}

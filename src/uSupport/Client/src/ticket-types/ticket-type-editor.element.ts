import { customElement, html, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { TicketType } from '../repository/usupport.repository.js';
import { USUPPORT_LOOKUP_WORKSPACE_CONTEXT, type USupportLookupWorkspaceContext } from '../shared/lookup-workspace.context.js';
import '../shared/lookup-editor.element.js';

@customElement('usupport-ticket-type-editor')
export class USupportTicketTypeEditorElement extends UmbLitElement {
	@state() private _model?: TicketType;

	#context?: USupportLookupWorkspaceContext<TicketType>;

	constructor() {
		super();
		this.consumeContext(USUPPORT_LOOKUP_WORKSPACE_CONTEXT, (context) => {
			this.#context = context as unknown as USupportLookupWorkspaceContext<TicketType>;
			this.observe(context?.model, (model) => (this._model = model as TicketType | undefined));
		});
	}

	#update = (partial: Partial<TicketType>) => this.#context?.updateModel(partial);

	#text = (key: keyof TicketType) => (e: InputEvent) => this.#update({ [key]: (e.target as HTMLInputElement).value });

	render() {
		const model = this._model;
		return html`
			<usupport-lookup-editor with-icon>
				${model
					? html`
							<uui-box headline=${this.localize.term('uSupport_customProperty')}>
								<umb-property-layout
									label=${this.localize.term('uSupport_dataType')}
									description=${this.localize.term('uSupport_dataTypeDescription')}>
									<umb-data-type-input
										slot="editor"
										max="1"
										.value=${model.dataTypeKey ?? ''}
										@change=${(e: Event) =>
											this.#update({ dataTypeKey: (e.target as HTMLInputElement).value || null })}></umb-data-type-input>
								</umb-property-layout>
								<umb-property-layout label=${this.localize.term('uSupport_propertyName')}>
									<uui-input
										slot="editor"
										label=${this.localize.term('uSupport_propertyName')}
										.value=${model.propertyName ?? ''}
										@input=${this.#text('propertyName')}></uui-input>
								</umb-property-layout>
								<umb-property-layout label=${this.localize.term('uSupport_propertyDescription')}>
									<uui-input
										slot="editor"
										label=${this.localize.term('uSupport_propertyDescription')}
										.value=${model.propertyDescription ?? ''}
										@input=${this.#text('propertyDescription')}></uui-input>
								</umb-property-layout>
							</uui-box>
						`
					: ''}
			</usupport-lookup-editor>
		`;
	}
}

export default USupportTicketTypeEditorElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-ticket-type-editor': USupportTicketTypeEditorElement;
	}
}

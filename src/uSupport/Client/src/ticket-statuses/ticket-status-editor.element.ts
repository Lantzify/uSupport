import { customElement, html, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UUIBooleanInputEvent } from '@umbraco-cms/backoffice/external/uui';
import type { TicketStatus } from '../repository/usupport.repository.js';
import { USUPPORT_LOOKUP_WORKSPACE_CONTEXT, type USupportLookupWorkspaceContext } from '../shared/lookup-workspace.context.js';
import { STATUS_COLORS, statusTag } from '../shared/display.js';
import '../shared/lookup-editor.element.js';

@customElement('usupport-ticket-status-editor')
export class USupportTicketStatusEditorElement extends UmbLitElement {
	@state() private _model?: TicketStatus;

	#context?: USupportLookupWorkspaceContext<TicketStatus>;

	constructor() {
		super();
		this.consumeContext(USUPPORT_LOOKUP_WORKSPACE_CONTEXT, (context) => {
			this.#context = context as unknown as USupportLookupWorkspaceContext<TicketStatus>;
			this.observe(context?.model, (model) => (this._model = model as TicketStatus | undefined));
		});
	}

	#update = (partial: Partial<TicketStatus>) => this.#context?.updateModel(partial);

	render() {
		const model = this._model;
		return html`
			<usupport-lookup-editor>
				${model
					? html`
							<uui-box>
								<umb-property-layout label=${this.localize.term('uSupport_color')}>
									<div slot="editor">
										<uui-select
											label=${this.localize.term('uSupport_color')}
											.options=${STATUS_COLORS.map((color) => ({ name: color, value: color, selected: color === model.color }))}
											@change=${(e: Event) => this.#update({ color: (e.target as HTMLSelectElement).value })}></uui-select>
										${statusTag(model)}
									</div>
								</umb-property-layout>
								<umb-property-layout
									label=${this.localize.term('uSupport_active')}
									description=${this.localize.term('uSupport_activeDescription')}>
									<uui-toggle
										slot="editor"
										label=${this.localize.term('uSupport_active')}
										?checked=${model.active}
										@change=${(e: UUIBooleanInputEvent) => this.#update({ active: e.target.checked })}></uui-toggle>
								</umb-property-layout>
								<umb-property-layout
									label=${this.localize.term('uSupport_default')}
									description=${this.localize.term('uSupport_defaultDescription')}>
									<uui-toggle
										slot="editor"
										label=${this.localize.term('uSupport_default')}
										?checked=${model.default}
										@change=${(e: UUIBooleanInputEvent) => this.#update({ default: e.target.checked })}></uui-toggle>
								</umb-property-layout>
							</uui-box>
						`
					: ''}
			</usupport-lookup-editor>
		`;
	}
}

export default USupportTicketStatusEditorElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-ticket-status-editor': USupportTicketStatusEditorElement;
	}
}

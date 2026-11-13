import { css, customElement, html, nothing, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import type { UUIBooleanInputEvent } from '@umbraco-cms/backoffice/external/uui';
import { USupportRepository, type Settings } from '../repository/usupport.repository.js';

type TextKey = 'ticketUpdateEmail' | 'emailSubjectNewTicket' | 'emailSubjectUpdateTicket' | 'emailTemplateNewTicketPath' | 'emailTemplateUpdateTicketPath';
type ToggleKey = 'sendEmailOnTicketCreated' | 'sendEmailOnTicketComment';

/** Email settings, stored in the uSupportSettings table. */
@customElement('usupport-settings-view')
export class USupportSettingsViewElement extends UmbLitElement {
	@state() private _settings?: Settings;
	@state() private _saving = false;

	#repository = new USupportRepository(this);

	async connectedCallback() {
		super.connectedCallback();
		this._settings = await this.#repository.getSettings();
	}

	async #save() {
		if (!this._settings) return;
		this._saving = true;
		const saved = await this.#repository.updateSettings(this._settings);
		this._saving = false;
		if (!saved) return;

		this._settings = saved;
		(await this.getContext(UMB_NOTIFICATION_CONTEXT))?.peek('positive', {
			data: { message: this.localize.term('speechBubbles_editContentSavedHeader') },
		});
	}

	#toggle(key: ToggleKey) {
		return html`
			<umb-property-layout label=${this.localize.term(`uSupport_${key}`)}>
				<uui-toggle
					slot="editor"
					label=${this.localize.term(`uSupport_${key}`)}
					?checked=${this._settings![key]}
					@change=${(e: UUIBooleanInputEvent) => (this._settings = { ...this._settings!, [key]: e.target.checked })}></uui-toggle>
			</umb-property-layout>
		`;
	}

	#text(key: TextKey) {
		return html`
			<umb-property-layout
				label=${this.localize.term(`uSupport_${key}`)}
				description=${this.localize.term(`uSupport_${key}Description`)}>
				<uui-input
					slot="editor"
					label=${this.localize.term(`uSupport_${key}`)}
					.value=${this._settings![key] ?? ''}
					@input=${(e: InputEvent) => (this._settings = { ...this._settings!, [key]: (e.target as HTMLInputElement).value })}></uui-input>
			</umb-property-layout>
		`;
	}

	render() {
		if (!this._settings) return html`<uui-loader-bar></uui-loader-bar>`;

		return html`
			<uui-box headline=${this.localize.term('uSupport_emailSettings')}>
				${this.#toggle('sendEmailOnTicketCreated')} ${this.#toggle('sendEmailOnTicketComment')}
				${this.#text('ticketUpdateEmail')} ${this.#text('emailSubjectNewTicket')} ${this.#text('emailSubjectUpdateTicket')}
				${this.#text('emailTemplateNewTicketPath')} ${this.#text('emailTemplateUpdateTicketPath')}
			</uui-box>
			<div class="actions">
				<uui-button
					look="primary"
					color="positive"
					label=${this.localize.term('buttons_save')}
					.state=${this._saving ? 'waiting' : undefined}
					@click=${this.#save}></uui-button>
			</div>
			${nothing}
		`;
	}

	static styles = css`
		:host {
			display: block;
			padding: var(--uui-size-layout-1);
		}
		uui-input {
			width: 100%;
		}
		.actions {
			display: flex;
			justify-content: flex-end;
			margin-top: var(--uui-size-space-4);
		}
	`;
}

export default USupportSettingsViewElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-settings-view': USupportSettingsViewElement;
	}
}

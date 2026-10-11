import { css, customElement, html, nothing, property, state, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { USupportRepository, type TicketComment } from '../repository/usupport.repository.js';
import { USUPPORT_RICH_TEXT_DATA_TYPE_KEY, markupOf } from '../constants.js';
import { formatDate } from '../shared/display.js';
import type { USupportTicketPropertyElement } from './ticket-property.element.js';
import './ticket-property.element.js';

/** Paged comment timeline for a ticket, with a rich text box to add a comment. */
@customElement('usupport-comments')
export class USupportCommentsElement extends UmbLitElement {
	@property()
	ticketId?: string;

	@state() private _comments: TicketComment[] = [];
	@state() private _page = 1;
	@state() private _totalPages = 1;
	@state() private _draft: string | null = null;
	@state() private _saving = false;

	#repository = new USupportRepository(this);

	protected updated(changed: Map<string, unknown>) {
		if (changed.has('ticketId') && this.ticketId) this.#load();
	}

	async #load() {
		if (!this.ticketId) return;
		const data = await this.#repository.getComments(this.ticketId, this._page);
		this._comments = data?.items ?? [];
		this._totalPages = Math.max(1, data?.totalPages ?? 1);
	}

	async #submit() {
		const markup = markupOf(this._draft);
		if (!this.ticketId || !markup) return;

		this._saving = true;
		const comment = await this.#repository.createComment(this.ticketId, markup);
		this._saving = false;
		if (!comment) return;

		this._draft = null;
		this._page = 1;
		await this.#load();
		(await this.getContext(UMB_NOTIFICATION_CONTEXT))?.peek('positive', {
			data: { message: this.localize.term('uSupport_commentAdded') },
		});
		this.dispatchEvent(new CustomEvent('comment-added', { bubbles: true, composed: true }));
	}

	render() {
		return html`
			${this._comments.map(
				(comment) => html`
					<div class="comment">
						<uui-avatar name=${comment.user?.name ?? '?'} img-src=${comment.user?.avatarUrls?.[0] ?? ''}></uui-avatar>
						<div>
							<strong>${comment.user?.name}</strong>
							<small>${formatDate(comment.date)}</small>
							<div class="body">${unsafeHTML(comment.comment)}</div>
						</div>
					</div>
				`,
			)}
			${this._totalPages > 1
				? html`<uui-pagination
						.current=${this._page}
						.total=${this._totalPages}
						@change=${(e: Event) => {
							this._page = (e.target as HTMLElement & { current: number }).current;
							this.#load();
						}}></uui-pagination>`
				: nothing}

			<usupport-ticket-property
				.dataTypeKey=${USUPPORT_RICH_TEXT_DATA_TYPE_KEY}
				label=${this.localize.term('uSupport_comment')}
				.value=${this._draft}
				@change=${(e: Event) => (this._draft = (e.target as USupportTicketPropertyElement).value ?? null)}></usupport-ticket-property>
			<div class="actions">
				<uui-button
					look="primary"
					color="positive"
					label=${this.localize.term('uSupport_comment')}
					.state=${this._saving ? 'waiting' : undefined}
					?disabled=${!markupOf(this._draft)}
					@click=${this.#submit}></uui-button>
			</div>
		`;
	}

	static styles = css`
		:host {
			display: block;
		}
		.comment {
			display: flex;
			gap: var(--uui-size-space-4);
			padding: var(--uui-size-space-4) 0;
			border-bottom: 1px solid var(--uui-color-border);
		}
		.comment small {
			display: block;
			color: var(--uui-color-text-alt);
		}
		.body {
			margin-top: var(--uui-size-space-2);
		}
		.actions {
			display: flex;
			justify-content: flex-end;
		}
	`;
}

declare global {
	interface HTMLElementTagNameMap {
		'usupport-comments': USupportCommentsElement;
	}
}

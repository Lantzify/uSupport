import { css, customElement, html } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

@customElement('usupport-admin-dashboard')
export class USupportAdminDashboardElement extends UmbLitElement {
	render() {
		return html`
			<uui-box headline="uSupport">
				<h4>What is uSupport</h4>
				<p>
					uSupport is a feature rich ticketing system designed specifically for Umbraco. It lets users create, manage
					and view tickets and their comments, with your own ticket types and statuses.
				</p>

				<h4>Why uSupport</h4>
				<p>
					Instead of managing your clients' Umbraco issues via email, keep them in Umbraco. No more rummaging through old
					emails to find the details of what needs to be fixed.
				</p>

				<h4>Getting started</h4>
				<ul>
					<li>
						<strong>Documentation:</strong>
						<a href="https://lantzify.github.io/uSupport-documentation/" target="_blank" rel="noopener">setup guides and best practices</a>
					</li>
					<li>
						<strong>uSync integration:</strong>
						<a href="https://github.com/Lantzify/uSupport-uSync" target="_blank" rel="noopener">uSupport uSync</a>
					</li>
					<li>
						<strong>Email templates:</strong>
						<a href="https://github.com/Lantzify/uSupport/raw/refs/heads/main/assets/email-templates.zip" target="_blank" rel="noopener"
							>download the email templates</a
						>
					</li>
					<li>
						<strong>Source code:</strong>
						<a href="https://github.com/Lantzify/uSupport" target="_blank" rel="noopener">GitHub</a>
					</li>
				</ul>
			</uui-box>
		`;
	}

	static styles = [
		UmbTextStyles,
		css`
			:host {
				display: block;
				padding: var(--uui-size-layout-1);
			}
		`,
	];
}

export default USupportAdminDashboardElement;

declare global {
	interface HTMLElementTagNameMap {
		'usupport-admin-dashboard': USupportAdminDashboardElement;
	}
}

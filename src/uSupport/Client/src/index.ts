import type { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { client } from './api/client.gen.js';
import { manifests } from './manifests.js';

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
	extensionRegistry.registerMany(manifests);

	host.consumeContext(UMB_AUTH_CONTEXT, (auth) => {
		if (!auth) return;
		const config = auth.getOpenApiConfiguration();
		client.setConfig({ auth: config.token, baseUrl: config.base, credentials: config.credentials });
		client.interceptors.request.use(async (request) => {
			request.headers.set('Authorization', `Bearer ${await auth.getLatestToken()}`);
			return request;
		});
	});
};

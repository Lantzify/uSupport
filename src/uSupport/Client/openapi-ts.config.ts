import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
	input: 'https://localhost:44311/umbraco/swagger/uSupport/swagger.json',
	output: { path: 'src/api' },
	plugins: [{ name: '@hey-api/sdk', asClass: true, classNameBuilder: '{{name}}Service' }],
});

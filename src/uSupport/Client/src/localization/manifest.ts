export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'localization',
		alias: 'uSupport.Localization.En',
		name: 'uSupport English',
		meta: { culture: 'en' },
		js: () => import('./en.js'),
	},
    {
		type: 'localization',
		alias: 'uSupport.Localization.Ja',
		name: 'uSupport Japanese',
		meta: { culture: 'ja' },
		js: () => import('./ja.js'),
	},
];

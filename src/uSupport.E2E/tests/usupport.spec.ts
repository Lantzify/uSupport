import { expect } from '@playwright/test';
import { test } from '@umbraco-cms/acceptance-test-helpers';

test('uSupport section exists', { tag: '@smoke' }, async ({ umbracoUi }) => {
  await umbracoUi.goToBackOffice();
  await expect(umbracoUi.page.getByRole('tab', { name: 'uSupport' })).toBeVisible();
});
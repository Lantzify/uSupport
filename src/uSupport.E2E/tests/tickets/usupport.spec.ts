import { expect } from '@playwright/test';
import { test } from '@umbraco-cms/acceptance-test-helpers';

test('uSupport create ticket', async ({ page }) => {
  await page.goto('https://localhost:44311/umbraco');
  await page.goto('https://localhost:44311/umbraco/section/content');
  await page.getByRole('tab', { name: 'Support', exact: true }).click();
  await page.getByRole('button', { name: 'Create ticket' }).click();
  await page.getByRole('button', { name: 'System error Umbraco isn\'t' }).click();
  await page.getByRole('textbox', { name: 'Title' }).click();
  await page.getByRole('textbox', { name: 'Title' }).fill('Test ticket');
  await page.getByTestId('input:tiptap-rte').getByRole('paragraph').click();
  await page.getByRole('textbox', { name: 'Summary' }).fill('Something is not working');
  await page.locator('#container').getByRole('button', { name: 'Create ticket' }).click();
  await expect(page.locator('#toast > div')).toBeVisible();
});
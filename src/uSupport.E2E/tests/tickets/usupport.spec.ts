import { expect } from '@playwright/test';
import { test } from '@umbraco-cms/acceptance-test-helpers';

test('uSupport create system error ticket', async ({ page }) => {
  await page.goto('https://localhost:44311/umbraco');
  await page.goto('https://localhost:44311/umbraco/section/content');
  await page.getByRole('tab', { name: 'Support', exact: true }).click();
  await page.getByRole('button', { name: 'Create ticket' }).click();
  await page.getByRole('button', { name: 'System error Umbraco isn\'t' }).click();
  await page.getByRole('textbox', { name: 'Title' }).fill('Test ticket');
  await page.getByRole('textbox', { name: 'Summary' }).fill('Something is not working');
  await page.locator('#container').getByRole('button', { name: 'Create ticket' }).click();
  await expect(page.locator('#toast > div')).toBeVisible();
});

test('uSupport create page error ticket', async ({ page }) => {
  await page.goto('https://localhost:44311/umbraco');
  await page.goto('https://localhost:44311/umbraco/section/content');
  await page.getByRole('tab', { name: 'Support', exact: true }).click(); 
  await page.getByRole('button', { name: 'Create ticket' }).click();
  await page.getByRole('button', { name: 'Page error A page isn\'t' }).click();
  await page.getByRole('button', { name: 'Choose' }).click();
  await page.getByRole('button', { name: 'Home', exact: true }).click();
  await page.locator('umb-tree-picker-modal').getByRole('button', { name: 'Choose' }).click();
  await page.getByRole('textbox', { name: 'Title' }).click();
  await page.getByRole('textbox', { name: 'Title' }).fill('Home page throws error');
  await page.getByRole('textbox', { name: 'Summary' }).fill('When going to the home page it throws a 500 error');
  await page.locator('#container').getByRole('button', { name: 'Create ticket' }).click();
  await expect(page.locator('#toast > div')).toBeVisible();
});

test('uSupport create general question ticket', async ({ page }) => {
  await page.goto('https://localhost:44311/umbraco');
  await page.goto('https://localhost:44311/umbraco/section/content');
  await page.getByRole('tab', { name: 'Support', exact: true }).click(); 
  await page.getByRole('button', { name: 'Create ticket' }).click();
  await page.getByRole('button', { name: 'General question' }).click();
  await page.getByRole('textbox', { name: 'Title' }).fill('Umbraco 18');
  await page.getByRole('textbox', { name: 'Summary' }).fill('When can we upgrade to Umbraco 18?');
  await page.locator('#container').getByRole('button', { name: 'Create ticket' }).click();
  await expect(page.locator('#toast > div')).toBeVisible();
});
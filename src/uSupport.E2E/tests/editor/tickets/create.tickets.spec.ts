import { expect } from '@playwright/test';
import { test } from '@umbraco-cms/acceptance-test-helpers';
import { TICKETS, createTicket } from '../../helpers';

test('System error ticket', async ({ page }) => {
  await createTicket(page, TICKETS.systemError.type,  TICKETS.systemError.title,  TICKETS.systemError.summary);
});

test('General question ticket', async ({ page }) => {
  await createTicket(page, TICKETS.generalQuestion.type, TICKETS.generalQuestion.title, TICKETS.generalQuestion.summary);
});

test('Page error ticket', async ({ page }) => {
  await page.goto('/umbraco/section/content');
  await page.getByRole('tab', { name: 'Support', exact: true }).click();
  await page.getByRole('button', { name: 'Create ticket' }).click();
  await page.getByRole('button', { name: TICKETS.pageError.type }).click();
  
  await page.getByRole('button', { name: 'Choose', exact: true }).click();
  await page.getByRole('button', { name: 'Home', exact: true }).click();
  await page.locator('umb-tree-picker-modal').getByRole('button', { name: 'Choose' }).click();
  await page.getByRole('textbox', { name: 'Title' }).click();
  await page.getByRole('textbox', { name: 'Title' }).fill(TICKETS.pageError.title);
  await page.getByRole('textbox', { name: 'Summary' }).fill(TICKETS.pageError.summary);
  await page.locator('#container').getByRole('button', { name: 'Create ticket' }).click();
  await expect(page.locator('uui-toast-notification').getByText(/was created/)).toBeVisible();
});

test('Add comment to ticket', async ({ page }) => {
  await page.goto('/umbraco/section/content');
  await page.getByRole('tab', { name: 'Support', exact: true }).click();

  await page.getByRole('cell', { name: 'Home page throws error' }).first().click();
  await page.getByRole('textbox', { name: 'Comment' }).fill('Adding a comment!');
  
  await page.getByRole('button', { name: 'Comment' }).click();

  await expect(page.getByText('Adding a comment!')).toBeVisible();
  await expect(page.locator('uui-toast-notification').getByText('Comment added')).toBeVisible();
});
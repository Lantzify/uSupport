import { expect } from '@playwright/test';
import { test } from '@umbraco-cms/acceptance-test-helpers';
import { TYPES, createTicket } from '../../helpers';

test('No umbraco property', async ({ page }) => {
    const type = `${TYPES.request.name} ${Date.now()}`;
    const ticket = `A request ${Date.now()}`;

    await page.goto('/umbraco/section/usupport/workspace/usupport-ticket-type/create');
    await page.getByRole('textbox', { name: 'Enter a name...' }).fill(type);
    await page.getByRole('textbox', { name: 'Enter a description...' }).fill(TYPES.request.summary);

    await page.getByRole('button', { name: 'Save' }).click();
    await expect(page.locator('uui-toast-notification').getByText('Type saved')).toBeVisible();

    await createTicket(page, type, ticket, TYPES.request.summary);

    await expect(page.locator('usupport-tickets-table[list="active"]').getByRole('cell', { name: ticket })).toBeVisible();
});

test('Umbraco property', async ({ page }) => {
    const type = `${TYPES.meetingRequest.name} ${Date.now()}`;
    const ticket = `Serious meeting ${Date.now()}`;

    await page.goto('/umbraco/section/usupport/workspace/usupport-ticket-type/create');
    await page.getByRole('textbox', { name: 'Enter a name...' }).fill(type);
    await page.getByRole('textbox', { name: 'Enter a description...' }).fill(TYPES.meetingRequest.summary);

    await page.getByRole('button', { name: 'Choose' }).click();
    await page.getByRole('button', { name: 'Date Picker with time' }).click();
    await page.locator('#container').getByRole('button', { name: 'Choose' }).click();
    await page.getByRole('textbox', { name: 'Property name' }).click();
    await page.getByRole('textbox', { name: 'Property name' }).fill('Date');

    await page.getByRole('button', { name: 'Save' }).click();
    await expect(page.locator('uui-toast-notification').getByText('Type saved')).toBeVisible();

    await page.goto('/umbraco/section/content');
    await page.getByRole('tab', { name: 'Support', exact: true }).click();
    await page.getByRole('button', { name: 'Create ticket' }).click();
    await page.getByRole('button', { name: type }).click();
    
    await page.getByRole('textbox', { name: 'Title' }).click();
    await page.getByRole('textbox', { name: 'Title' }).fill(ticket);
    await page.getByRole('textbox', { name: 'Summary' }).fill("Let's have a serious meeting.");


    await page.getByRole('textbox', { name: 'Set a date...' }).fill('2026-11-18T13:17:50');
    await page.locator('#container').getByRole('button', { name: 'Create ticket' }).click();
    await expect(page.locator('uui-toast-notification').getByText(/was created/)).toBeVisible();

    await expect(page.locator('usupport-tickets-table[list="active"]').getByRole('cell', { name: ticket })).toBeVisible();
});
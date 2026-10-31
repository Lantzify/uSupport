import { expect } from '@playwright/test';
import { test } from '@umbraco-cms/acceptance-test-helpers';
import { TICKETS, STATUSES, createTicket, TICKETS_OVERVIEW } from '../../helpers';


test('Wont fix status', async ({ page }) => {
    const status = `${STATUSES.wontFix.name} ${Date.now()}`;
    const ticket = `Headless ${Date.now()}`;

    await page.goto('/umbraco/section/usupport/workspace/usupport-ticket-status/create');
    await page.getByRole('textbox', { name: 'Enter a name...' }).fill(status);
    await page.getByRole('button', { name: 'Save' }).click();
    await expect(page.locator('uui-toast-notification').getByText('Status saved')).toBeVisible();

    await createTicket(page, TICKETS.generalQuestion.type, ticket, 'Can we use headless instead?');
    await page.getByRole('cell', { name: ticket }).click();

    await page.getByTestId('workspace:usupport-ticket').getByLabel('Status').selectOption(status);
    await page.getByRole('button', { name: 'Save', exact: true }).click();

    await expect(page.locator('uui-toast-notification').getByText('Ticket saved')).toBeVisible();
    await expect(page.locator('uui-tag')).toHaveText(status);

    await page.goto(TICKETS_OVERVIEW); 

    await expect(page.locator('usupport-tickets-table[list="active"]').getByRole('cell', { name: ticket })).toBeVisible();
    await expect(page.locator('usupport-tickets-table[list="resolved"]').getByRole('cell', { name: ticket })).not.toBeVisible();
});

test('Done status', async ({ page }) => {
    const status = `${STATUSES.done.name} ${Date.now()}`;
    const ticket = `Headless ${Date.now()}`;

    await page.goto('/umbraco/section/usupport/workspace/usupport-ticket-status/create');
    await page.getByRole('textbox', { name: 'Enter a name...' }).fill(status);
    await page.locator('uui-toggle[label="Active"]').click();
    await expect(page.getByRole('switch', { name: 'Active' })).not.toBeChecked();
    await page.getByRole('button', { name: 'Save' }).click();

    await expect(page.locator('uui-toast-notification').getByText('Status saved')).toBeVisible();

    await createTicket(page, TICKETS.generalQuestion.type, ticket, 'Can we use headless instead?');
    await page.getByRole('cell', { name: ticket }).click();

    await page.getByTestId('workspace:usupport-ticket').getByLabel('Status').selectOption(status);
    await page.getByRole('button', { name: 'Save', exact: true }).click();

    await expect(page.locator('uui-toast-notification').getByText('Ticket saved')).toBeVisible();
    await expect(page.locator('uui-tag')).toHaveText(status);

    await page.goto(TICKETS_OVERVIEW); 
    
    await expect(page.locator('usupport-tickets-table[list="active"]').getByRole('cell', { name: ticket })).not.toBeVisible();
    await expect(page.locator('usupport-tickets-table[list="resolved"]').getByRole('cell', { name: ticket })).toBeVisible();
});
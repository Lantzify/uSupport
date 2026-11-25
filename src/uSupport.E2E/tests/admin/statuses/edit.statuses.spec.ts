import { expect } from '@playwright/test';
import { test } from '@umbraco-cms/acceptance-test-helpers';

// uSupportConstants.NewTicketStatusGuid, the seeded default
const NEW_ID = '0fa7b428-efad-4617-9253-0ac162d9c672';
// uSupportConstants.InProgressTicketStatusGuid, seeded without an icon
const IN_PROGRESS_ID = '9b7b466c-7f61-46b1-bb1b-28986e295031';

test('Save built-in status', async ({ page }) => {
    await page.goto(`/umbraco/section/usupport/workspace/usupport-ticket-status/edit/${IN_PROGRESS_ID}`);
    await page.getByRole('button', { name: 'Save' }).click();
    await expect(page.locator('uui-toast-notification').getByText('Status saved')).toBeVisible();
});

test('Default status cannot be turned off or deleted', async ({ page }) => {
    const status = `Default ${Date.now()}`;
    const toast = page.locator('uui-toast-notification');
    const defaultToggle = page.locator('uui-toggle[label="Default"]');
    const save = page.getByRole('button', { name: 'Save' });

    // A new default has no tickets, so only being the default can stop the delete
    await page.goto('/umbraco/section/usupport/workspace/usupport-ticket-status/create');
    await page.getByRole('textbox', { name: 'Enter a name...' }).fill(status);
    await defaultToggle.click();
    await save.click();
    await expect(toast.getByText('Status saved')).toBeVisible();
    await page.waitForURL(/\/edit\//);
    await page.reload(); // the editor refetches after create, which would undo the next click

    await defaultToggle.click();
    await save.click();
    await expect(toast.getByText('Make another status the default instead')).toBeVisible();

    await page.reload();
    await expect(page.getByRole('switch', { name: 'Default' })).toBeChecked();
    // The name field takes focus a moment after loading, which closes an open menu
    await expect(async () => {
        await page.getByTestId('workspace:action-menu-button').click();
        await page.getByTestId('entity-action:uSupport.TicketStatus.EntityAction.Delete').click({ timeout: 1000 });
    }).toPass();
    await page.locator('umb-confirm-modal').getByRole('button', { name: 'Delete' }).click();
    await expect(toast.getByText('Make another status the default first')).toBeVisible();

    // New takes over as the default again
    await page.goto(`/umbraco/section/usupport/workspace/usupport-ticket-status/edit/${NEW_ID}`);
    await defaultToggle.click();
    await save.click();
    await expect(toast.getByText('Status saved')).toBeVisible();
});

import { expect } from '@playwright/test';
import { test } from '@umbraco-cms/acceptance-test-helpers';

// uSupportConstants.InProgressTicketStatusGuid, seeded without an icon
const IN_PROGRESS_ID = '9b7b466c-7f61-46b1-bb1b-28986e295031';

test('Save built-in status', async ({ page }) => {
    await page.goto(`/umbraco/section/usupport/workspace/usupport-ticket-status/edit/${IN_PROGRESS_ID}`);
    await page.getByRole('button', { name: 'Save' }).click();
    await expect(page.locator('uui-toast-notification').getByText('Status saved')).toBeVisible();
});

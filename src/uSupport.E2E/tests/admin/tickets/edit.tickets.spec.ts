import { expect } from '@playwright/test';
import { test } from '@umbraco-cms/acceptance-test-helpers';
import { TICKETS, TICKETS_OVERVIEW, createTicket } from '../../helpers';

const UNKNOWN_TYPE_ID = '11111111-1111-1111-1111-111111111111';

test('Ticket with an unknown type stays listed', async ({ page }) => {
  const ticket = `Unknown type ${Date.now()}`;
  const row = page.locator('usupport-tickets-table[list="active"]').getByRole('cell', { name: ticket });
  const type = page.getByTestId('workspace:usupport-ticket').getByLabel('Type');
  const save = page.getByRole('button', { name: 'Save', exact: true });

  await createTicket(page, TICKETS.generalQuestion.type, ticket);
  await row.click();
  await expect(type).toBeVisible(); // Save does nothing until the ticket has loaded

  // The editor only offers known types, so swap the type in the request it sends
  await page.route('**/usupport/ticket/*', (route) =>
    route.request().method() === 'PUT'
      ? route.continue({ postData: JSON.stringify({ ...route.request().postDataJSON(), typeId: UNKNOWN_TYPE_ID }) })
      : route.fallback(),
  );
  const response = page.waitForResponse((r) => r.request().method() === 'PUT');
  await save.click();
  expect((await response).request().postDataJSON().typeId).toBe(UNKNOWN_TYPE_ID);
  await page.unrouteAll();

  await page.goto(TICKETS_OVERVIEW);
  await row.click();

  // and it can be given a known type again
  await type.selectOption(TICKETS.systemError.type);
  await save.click();
  await expect(page.locator('uui-toast-notification').getByText('Ticket saved')).toBeVisible();
});

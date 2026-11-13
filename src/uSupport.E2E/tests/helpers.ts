import { Page, expect } from '@playwright/test';
import { ConstantHelper, UiHelpers } from '@umbraco-cms/acceptance-test-helpers';

export const TICKETS_OVERVIEW = '/umbraco/section/usupport/workspace/usupport-tickets-root/';

export const TICKETS = {
  generalQuestion: {
    type: 'General question',
    title: 'Umbraco 18',
    summary: 'When can we upgrade to Umbraco 18?'
  },
  systemError: {
    type: 'System error',
    title: 'Test ticket',
    summary: 'Something is not working'
  },
  pageError: {
    type: 'Page error',
    title: 'Home page throws error',
    summary: 'When going to the home page it throws a 500 error'
  }
}

export const STATUSES = {
  done: {
    name: "Done"
  },
  wontFix: {
    name: "Won't fix"
  }
};

/** Logs in through the backoffice UI and saves the session for a project's storageState. */
export async function login(page: Page, email: string, password: string, statePath: string) {
  const ui = new UiHelpers(page);
  await ui.goToBackOffice();
  await ui.login.enterEmail(email);
  await ui.login.enterPassword(password);
  await ui.login.clickLoginButton();
  // false = don't require every section tab, editors don't have Settings etc.
  await ui.login.goToSection(ConstantHelper.sections.content, false);
  await page.context().storageState({ path: statePath });
}


export async function createTicket(page: Page, type: string, title: string, summary = 'Test summary') {
  await page.goto('/umbraco/section/content');
  await page.getByRole('tab', { name: 'Support', exact: true }).click();
  await page.getByRole('button', { name: 'Create ticket' }).click();
  await page.getByRole('button', { name: type }).click();
  await page.getByRole('textbox', { name: 'Title' }).fill(title);
  await page.getByRole('textbox', { name: 'Summary' }).fill(summary);
  await page.locator('#container').getByRole('button', { name: 'Create ticket' }).click();
  await expect(page.locator('uui-toast-notification').getByText(/was created/)).toBeVisible();
}
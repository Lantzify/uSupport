import { test as setup } from '@playwright/test';
import { ConstantHelper, UiHelpers } from '@umbraco-cms/acceptance-test-helpers';
import { STORAGE_STATE } from '../playwright.config';

setup('log in', async ({ page }) => {
  const ui = new UiHelpers(page);
  await ui.goToBackOffice();
  await ui.login.enterEmail(process.env.UMBRACO_USER_LOGIN!);
  await ui.login.enterPassword(process.env.UMBRACO_USER_PASSWORD!);
  await ui.login.clickLoginButton();
  await ui.login.goToSection(ConstantHelper.sections.settings);
  await page.context().storageState({ path: STORAGE_STATE });
});
import { test as setup } from '@playwright/test';
import { ApiHelpers } from '@umbraco-cms/acceptance-test-helpers';
import { EDITOR_STATE } from '../../playwright.config';
import { login } from '../helpers';

setup('log in as editor', async ({ page, browser }) => {
  const email = process.env.UMBRACO_EDITOR_LOGIN!;
  const password = process.env.UMBRACO_EDITOR_PASSWORD!;

  // Unattended install only creates the admin, so log in as admin over the API to create the editor
  const adminContext = await browser.newContext();
  const api = new ApiHelpers(await adminContext.newPage());
  await api.updateTokenAndCookie(process.env.UMBRACO_USER_LOGIN!, process.env.UMBRACO_USER_PASSWORD!);
  const editors = await api.userGroup.getByName('Editors');
  // Creates the user if missing and resets the password, so reruns are safe
  await api.user.setUserPermissions('Editor', email, password, editors.id);
  await adminContext.close();

  await login(page, email, password, EDITOR_STATE);
});

import { test as setup } from '@playwright/test';
import { ADMIN_STATE } from '../../playwright.config';
import { login } from '../helpers';

setup('log in as admin', async ({ page }) => {
  await login(page, process.env.UMBRACO_USER_LOGIN!, process.env.UMBRACO_USER_PASSWORD!, ADMIN_STATE);
});

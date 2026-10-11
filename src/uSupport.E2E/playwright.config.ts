import path from 'path';
import { defineConfig, devices } from '@playwright/test';

// Must match Umbraco:CMS:Unattended in appsettings and the https URL in launchSettings.json
process.env.URL = 'https://localhost:44311';
process.env.UMBRACO_USER_LOGIN = 'admin@admin.com';
process.env.UMBRACO_USER_PASSWORD = 'password123';

export const STORAGE_STATE = path.join(__dirname, 'tests/.auth/user.json');
process.env.STORAGE_STAGE_PATH = STORAGE_STATE;

export default defineConfig({
  testDir: './tests',
  workers: 1,
  retries: process.env.CI ? 2 : 0,
  reporter: 'html',
  use: {
    baseURL: process.env.URL,
    ignoreHTTPSErrors: true,
    testIdAttribute: 'data-mark',
    trace: 'retain-on-failure',
  },
  projects: [
    { name: 'setup', testMatch: /.*\.setup\.ts/ },
    {
      name: 'chromium',
      testMatch: /.*\.spec\.ts/,
      dependencies: ['setup'],
      use: { ...devices['Desktop Chrome'], storageState: STORAGE_STATE },
    },
  ],
});
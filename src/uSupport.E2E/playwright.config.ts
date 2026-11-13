import path from 'path';
import { defineConfig, devices } from '@playwright/test';


process.env.URL = 'https://localhost:44318';

//Admin
process.env.UMBRACO_USER_LOGIN = 'admin@admin.com';
process.env.UMBRACO_USER_PASSWORD = 'password123';

// Editor
process.env.UMBRACO_EDITOR_LOGIN = 'editor@editor.com';
process.env.UMBRACO_EDITOR_PASSWORD = 'password123';

export const ADMIN_STATE = path.join(__dirname, 'tests/.auth/admin.json');
export const EDITOR_STATE = path.join(__dirname, 'tests/.auth/editor.json');

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
  // Starts the site if nothing is running on the URL yet; locally it reuses IIS Express from Visual Studio
  webServer: {
    command: `dotnet run --project ../uSupport.Web -- --urls ${process.env.URL}`,
    url: `${process.env.URL}/umbraco`,
    ignoreHTTPSErrors: true,
    reuseExistingServer: !process.env.CI,
    timeout: 5 * 60_000, // first boot runs the unattended install
  },
  projects: [
    { name: 'admin-setup', testMatch: 'admin.setup.ts' },
    { name: 'editor-setup', testMatch: 'editor.setup.ts' },
    {
      name: 'admin',
      testMatch: '*.spec.ts',
      testIgnore: 'editor/**',
      dependencies: ['admin-setup'],
      use: { ...devices['Desktop Chrome'], storageState: ADMIN_STATE },
    },
    {
      name: 'editor',
      testMatch: 'editor/**/*.spec.ts',
      dependencies: ['editor-setup'],
      use: { ...devices['Desktop Chrome'], storageState: EDITOR_STATE },
    },
  ],
});
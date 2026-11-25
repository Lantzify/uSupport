# uSupport

Ticketing system for the Umbraco backoffice, shipped as the `uSupport` NuGet package (MIT, by Lantzify). Editors raise tickets from a dashboard in the Content section; support staff with the uSupport section manage tickets, comments, ticket types, ticket statuses and email settings.

Current line: **v17** (Umbraco 17.5, .NET 10, Lit backoffice). The old AngularJS line is v13 (`v13/main`). Version numbers follow the Umbraco major (package 17.x targets Umbraco 17).

More detail lives next to the code and loads automatically when you work there:
- [src/uSupport/CLAUDE.md](src/uSupport/CLAUDE.md): backend (controllers, services, NPoco, migrations, notifications, email)
- [src/uSupport/Client/CLAUDE.md](src/uSupport/Client/CLAUDE.md): backoffice frontend (Lit, manifests, generated API client)

## Repository layout

```
src/
  uSupport.slnx                 solution (package, demo site, unit tests)
  package.json                  shortcuts: install-client, build-watch, generate
  uSupport/                     THE PACKAGE (Razor class library, packs to NuGet)
    Client/                     backoffice frontend source, Vite builds it into ../wwwroot (gitignored)
    Controllers/ Services/ Migrations/ Notifications/ Dtos/ Helpers/ Composers/ Constants/ Extensions/
    Views/Partials/uSupport/Emails/   default Razor email templates
  uSupport.Web/                 demo Umbraco site (Clean starter kit, SQLite, unattended install) for dev + E2E
  uSupport.Tests/               NUnit unit tests (pure logic only, no database)
  uSupport.E2E/                 Playwright tests against uSupport.Web
assets/                         README screenshots, logo, email-templates.zip
umbraco-marketplace.json        marketplace listing; umbraco-marketplace-readme.md is the NuGet readme
```

Related repos (they consume this package's public notifications and API, so keep those stable): [uSupport-Automate](https://github.com/Lantzify/uSupport-Automate), [uSupport-uSync](https://github.com/Lantzify/uSupport-uSync). User docs: https://lantzify.github.io/uSupport-documentation/

## Commands

Run from the repo root unless noted.

| Task | Command |
|------|---------|
| Install frontend deps | `cd src/uSupport/Client && npm ci` |
| Build frontend (required before the .NET build ships any UI) | `cd src/uSupport/Client && npm run build` |
| Watch frontend while the site runs | `cd src/uSupport/Client && npm run watch` |
| Regenerate API client (site must be running on https://localhost:44311) | `cd src/uSupport/Client && npm run generate` |
| Run demo site | `dotnet run --project src/uSupport.Web` (Kestrel https://localhost:44311; IIS Express uses 44318) |
| Unit tests | `dotnet test src/uSupport.Tests` |
| E2E tests | `cd src/uSupport.E2E && npm ci && npx playwright test` (starts the site on 44318 if not running) |
| Pack | `dotnet pack -c Release src/uSupport /p:Version=17.x.y` (output: `src/uSupport/build/Release`) |

Demo site logins (unattended install, local only): admin `admin@admin.com` / `password123`; the E2E setup also creates editor `editor@editor.com` / `password123`.

## Branches and releases

- `v17/dev` is where work happens; PRs go to `v17/main`. `main` follows the latest major. `v13/main` is the legacy line.
- E2E runs on PRs to `v*/main`. Claude code review runs on PRs.
- Publishing a GitHub release runs `.github/workflows/main.yml`: builds the client, runs unit tests, packs with the release tag as version, pushes to NuGet.
- When bumping the version, update all of: `src/uSupport/uSupport.csproj` `<Version>`, `src/package.json`, `src/uSupport/Client/package.json`, `src/uSupport/Client/public/umbraco-package.json`.

## Feature workflow

- Trunk for planning is `v17/dev`. Each feature's plan lives in `docs/plans/<feature-slug>/` (BRIEF, ARCHITECTURE, SPEC, STORIES, PLAN, logs). The slug is also the branch name.
- Plan folders stay uncommitted until the feature is built. Closed plans move to `docs/plans/archive/<feature-slug>/`.

## Domain model

| Entity | Table | Notes |
|--------|-------|-------|
| Ticket | `uSupportTicket` | Title, Summary (sanitized HTML), TypeId, StatusId, AuthorId (Umbraco user int id), ExternalTicketId (human-readable id), PropertyValue (JSON string from the type's custom property), InternalComment (support-only), Submitted/Resolved/LastUpdated |
| Ticket type | `uSupportTicketType` | Sortable lookup. Optional custom property: an Umbraco data type (`DataTypeKey`) + name/description shown on the ticket |
| Ticket status | `uSupportTicketStatus` | Sortable lookup. `Default` = assigned to new tickets. `Active = false` means resolved |
| Comment | `uSupportTicketComment` | Sanitized HTML, per ticket |
| History | `uSupportTicketHistory` | Audit trail: Created, Updated, Resolved, Comment, SentEmail, with a JSON list of field changes |
| Settings | `uSupportSettings` | Single row: email toggles, support address, subject templates with `{Property.Path}` tokens, template view paths |

Seed GUIDs for the default types and statuses are in `src/uSupport/Constants/uSupportConstants.cs`.

## Who can do what

- Any backoffice user: see their dashboard in Content, list/read tickets, create tickets, comment.
- Users whose group has the `uSupport` section: everything else (update/delete tickets, manage types, statuses, settings). Enforced server-side by the `uSupportSectionAccess` policy; internal comments are stripped from responses for non-section users.
- The section alias must stay `uSupport`: v13 user groups already store it.

## Working rules

- Backend and frontend change together: a new or changed endpoint means `npm run generate` and committing the regenerated `Client/src/api/`.
- Existing installs upgrade in place. Schema changes go through a new migration step, never by editing an old one (see backend CLAUDE.md).
- Notifications and the REST API are a public contract for Automate/uSync. Add, don't rename or remove.
- Every UI string goes through localization (`en.ts` and `ja.ts`).
- Shortcuts are marked in code as `ponytail:` or `shortcut:` comments with their limit; keep that format.

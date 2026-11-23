# uSupport backend

Razor class library (`Microsoft.NET.Sdk.Razor`, net10.0) on Umbraco 17.5. Static web assets are served from `App_Plugins/uSupport`; the built frontend lands in `wwwroot/` (gitignored, built from `Client/`). Frontend notes: [Client/CLAUDE.md](Client/CLAUDE.md).

## Request flow

```
Controller (Controllers/)            validates, sanitizes HTML, checks auth, sends email, publishes notifications
  -> Service (Services/)             NPoco SQL against the uSupport tables, inside an Umbraco scope
      -> Schema (Migrations/Schemas/) table definition, used for insert/update
      <- Dto (Dtos/Tables/)           read model, may carry joined Type/Status/Author/Comments
```

Mapping between Dto and Schema is hand-written in `Helpers/uSupportTypeHelper.cs` (`ConvertDtoToSchema` / `ConvertSchemaToDto`). Add a column: add it to the Schema, the Dto, the converter, and a migration.

## Composers (all DI lives here)

- `uSupportServicesComposer`: registers services (transient), `uSupportUserHelper`, and the Swagger doc + operation/schema id handlers so the `uSupport` API gets Umbraco-style ids (`GetUsupportTicketById`), which the generated client depends on.
- `uSupportAuthorizationComposer`: the `uSupportSectionAccess` policy (user must have the `uSupport` section).
- `uSupportMigrationComposer`: runs the migration plan on `UmbracoApplicationStartingNotification`.
- `uSupportNotificationComposer`: wires `TicketHistoryNotificationHandler`.

## Controllers

All inherit `uSupportControllerBase` (`ManagementApiControllerBase`, `[MapToApi("uSupport")]`, API v1), routed with `[VersionedApiBackOfficeRoute("usupport/...")]`, so they sit under `/umbraco/management/api/v1/usupport/` with backoffice auth. Swagger: `/umbraco/swagger/uSupport/swagger.json`.

| Controller | Route | Notes |
|------------|-------|-------|
| `uSupportTicketController` | `usupport/ticket` | active/resolved paged lists (search + whitelisted sort), get, history, create, update, delete |
| `uSupportTicketCommentController` | `usupport/ticket-comment` | paged comments per ticket, create (emails the other party) |
| `uSupportTicketTypeController` | `usupport/ticket-type` | CRUD, sort, dependent-tickets preview; delete blocked while tickets use it |
| `uSupportTicketStatusController` | `usupport/ticket-status` | same shape as types |
| `uSupportSettingsController` | `usupport/settings` | whole controller requires section access |

Rules:
- Reads, ticket create and comment create are open to any backoffice user. Every mutation of shared data gets `[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]`.
- Sanitize user HTML with `IHtmlSanitizer` before storing (Summary, Comment).
- Strip `InternalComment` (and its history entries) for non-section users, as `uSupportTicketController` does.
- Return `NotFound()` / `Problem(title, 400)`; declare every response with `[ProducesResponseType]` so the generated client gets the types.
- After changing any endpoint or response model, regenerate the client (`npm run generate` in `Client/`).

## Services and data access

- `uSupportServiceBase<TDto, TSchema>` (in `Services/`, but namespace `uSupport.Services.Interfaces`) gives Get/GetByIds/Create/Update/Delete by Guid `Id`. Interfaces are in `Services/Interfaces/`.
- Scope pattern: `GetScope()` joins the ambient scope if there is one, else creates its own. Always use the try/finally shape already in the services: `Complete()` and `Dispose()` only when `context.Created`.
- SQL is NPoco `Sql` built by hand. GUIDs are stored upper-case, so compare with `UPPER('{id}')` (only ever interpolate typed `Guid`s, never strings). Free text goes in as parameters (`@0`). Sort columns go through the `SortableColumns` whitelist in `uSupportTicketService.GetOrderBy`.
- `SqlExtensions.GetFullTicket()` joins type and status onto a ticket query.
- Paging: `uSupportPageHelper.MapPageToUSupportPage`, page size `uSupportConstants.PageSize` (10).
- SQLite (demo site) and SQL Server must both work; keep SQL portable.

## Migrations

- Plan: `Migrations/uSupportMigrationPlan.cs`, plan name `uSupport`, run at startup when the runtime level is `Run`.
- Add a step: create a class in `Migrations/Updates/<version>/` inheriting `uSupportMigrationBase` (implement `DoMigrate()`), append `.To<YourStep>("unique-state-key")` at the END of the plan.
- Never rename, reorder or remove an existing step or its state key, including the `uSuppor-` typos: installed sites store the last key they ran.
- Make steps idempotent (`ColumnExists` / `TableExists` checks), like `AddInternalCommentColumn`.
- Pure conversion logic goes in a static method so it can be unit tested without a database (see `MigrateContentPickerValues.ToKeyJson` and its test).
- 17.0.0 steps convert v13 data: `AddDataTypeKeyToTicketTypes` (int data type id -> key) and `MigrateContentPickerValues` (node id / UDI -> document key).

## Notifications

Classes in `Notifications/` (Tickets, TicketTypes, TicketStatuses, Comments) are published through `IEventAggregator`. They are the extension point other packages (uSupport-Automate, uSupport-uSync) hook into: add new ones freely, don't change existing constructors or properties.

- `CreateTicketNotification` is published by `uSupportTicketService.Create`; everything else is published by controllers (update, resolved, send email, type/status CRUD, add comment).
- `TicketHistoryNotificationHandler` turns notifications into `uSupportTicketHistory` rows. A new tracked field on ticket update goes in its `Handle(TicketHistoryNotification)` diff.

## Email

- `uSupportSettingsService.SendEmail` renders a Razor view to a string and sends via Umbraco's `IEmailSender`. It needs `Umbraco:CMS:Global:Smtp:From` in appsettings; failures are logged, never thrown to the caller.
- Templates inherit `uSupport.Extensions.uSupportTemplate<uSupportTicket>` (gives `GetBaseUrl`). Defaults are in `Views/Partials/uSupport/Emails/`; sites can point settings at their own views.
- Subjects use `{Property.Path}` tokens resolved against the ticket by `uSupportTokenHelper.ReplaceTokens` (e.g. `{ExternalTicketId}`, `{Status.Name}`).
- Who gets mail: new ticket -> `TicketUpdateEmail` (comma-separated allowed); update with "send email" -> ticket author; comment -> author if someone else commented, otherwise `TicketUpdateEmail`.

## Tests

`src/uSupport.Tests` (NUnit 3). Only database-free logic is covered: token replacement, sort whitelist, picker migration conversion. Put new pure logic where it can be tested the same way; behaviour through the UI is covered by `src/uSupport.E2E`.

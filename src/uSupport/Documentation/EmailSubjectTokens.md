# Email Subject Token Replacement

The email subject fields in uSupport settings support dynamic token replacement using curly braces `{PropertyName}`.

## How to Use Tokens

Use `{PropertyName}` syntax in your email subject templates. The tokens will be automatically replaced with actual values when emails are sent.

### Example Configuration

```json
{
  "uSupport": {
    "Settings": {
      "Tickets": {
        "EmailSubjectNewTicket": "New Ticket: {ExternalTicketId} - {Title}",
        "EmailSubjectUpdateTicket": "Ticket Updated: {ExternalTicketId} | Status: {Status.Name}"
      }
    }
  }
}
```

## Available Tokens

### Basic Ticket Properties

| Token | Description | Example Value |
|-------|-------------|---------------|
| `{Id}` | Ticket GUID | `3fa85f64-5717-4562-b3fc-2c963f66afa6` |
| `{ExternalTicketId}` | Human-readable ticket ID | `ABC-1234-567` |
| `{Title}` | Ticket title | `Login page not working` |
| `{Summary}` | Ticket summary/description | `Users cannot log in...` |
| `{AuthorId}` | User ID of ticket creator | `1` |
| `{Submitted}` | Date/time ticket was created | `1/15/2024 10:30:00 AM` |
| `{Resolved}` | Date/time ticket was resolved (if resolved) | `1/20/2024 3:45:00 PM` |
| `{LastUpdated}` | Date/time of last update | `1/18/2024 2:15:00 PM` |
| `{LastUpdatedBy}` | Name of user who last updated | `Admin User` |
| `{PropertyValue}` | Custom property value | `https://example.com/page` |
| `{InternalComment}` | Internal admin comment | `Need to investigate server logs` |

### Nested Properties - Status

| Token | Description | Example Value |
|-------|-------------|---------------|
| `{Status.Name}` | Status display name | `Open`, `In Progress`, `Resolved` |
| `{Status.Alias}` | Status alias/key | `open`, `in-progress`, `resolved` |
| `{Status.Color}` | Status color code | `green`, `blue`, `gray` |
| `{Status.Icon}` | Status icon name | `icon-check`, `icon-loading` |
| `{Status.Active}` | Whether status is active | `True`, `False` |

### Nested Properties - Type

| Token | Description | Example Value |
|-------|-------------|---------------|
| `{Type.Name}` | Ticket type name | `Bug Report`, `Feature Request` |
| `{Type.Alias}` | Type alias/key | `bug-report`, `feature-request` |
| `{Type.Description}` | Type description | `Report a bug in the system` |
| `{Type.Color}` | Type color code | `red`, `blue`, `orange` |
| `{Type.Icon}` | Type icon name | `icon-bug`, `icon-lightbulb` |
| `{Type.PropertyName}` | Custom property field name | `Url`, `Description` |

### Nested Properties - Author (if populated)

| Token | Description | Example Value |
|-------|-------------|---------------|
| `{Author.Name}` | Ticket author's full name | `John Doe` |
| `{Author.Email}` | Author's email address | `john.doe@example.com` |

## Example Subject Templates

### New Ticket Notifications

```
Simple:
"New Ticket: {ExternalTicketId}"

Descriptive:
"New Ticket: {ExternalTicketId} - {Title}"

Detailed:
"{Type.Name} Ticket Created: {ExternalTicketId} - {Title}"

With Author:
"New ticket from {Author.Name}: {ExternalTicketId} - {Title}"
```

### Update Ticket Notifications

```
Simple:
"Ticket Updated: {ExternalTicketId}"

With Status:
"Ticket {ExternalTicketId} is now {Status.Name}"

Detailed:
"Ticket Updated: {ExternalTicketId} | Status: {Status.Name}"

Full Context:
"{Type.Name} #{ExternalTicketId}: '{Title}' - Status changed to {Status.Name}"

With Date:
"Ticket {ExternalTicketId} updated on {LastUpdated} by {LastUpdatedBy}"
```

## Important Notes

1. **Case Sensitivity**: Token names are case-sensitive. Use exact property names (e.g., `{Title}`, not `{title}`)

2. **Nested Properties**: Use dot notation for nested objects (e.g., `{Status.Name}`, `{Type.Color}`)

3. **Null Values**: If a property is null or doesn't exist, it will be replaced with an empty string

4. **Date Formatting**: DateTime properties use the default .NET `ToString()` format. If you need custom formatting, modify the template files instead

5. **Author Availability**: The `Author` property may not always be populated. Use with caution in templates

## Testing Your Templates

To test your email subject templates:

1. Update the settings in `appsettings.json`
2. Create or update a test ticket
3. Check the email subject line in the sent email
4. Verify all tokens are replaced correctly

## Troubleshooting

**Token not replaced**: Check spelling and case sensitivity
**Empty value**: Property might be null - check if the property is populated
**Nested property not working**: Ensure the parent object exists (e.g., `Status` must exist for `{Status.Name}`)

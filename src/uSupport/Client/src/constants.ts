/** Must stay "uSupport": user groups granted the section in v13 store this alias. */
export const USUPPORT_SECTION_ALIAS = 'uSupport';
export const USUPPORT_SECTION_PATHNAME = 'usupport';
export const USUPPORT_MENU_ALIAS = 'uSupport.Menu';

export const USUPPORT_TICKETS_ROOT_ENTITY_TYPE = 'usupport-tickets-root';
export const USUPPORT_TICKET_ENTITY_TYPE = 'usupport-ticket';
export const USUPPORT_TICKET_TYPE_ROOT_ENTITY_TYPE = 'usupport-ticket-type-root';
export const USUPPORT_TICKET_TYPE_ENTITY_TYPE = 'usupport-ticket-type';
export const USUPPORT_TICKET_STATUS_ROOT_ENTITY_TYPE = 'usupport-ticket-status-root';
export const USUPPORT_TICKET_STATUS_ENTITY_TYPE = 'usupport-ticket-status';
export const USUPPORT_SETTINGS_ENTITY_TYPE = 'usupport-settings';

export const USUPPORT_TICKETS_WORKSPACE_ALIAS = 'uSupport.Workspace.Tickets';
export const USUPPORT_TICKET_WORKSPACE_ALIAS = 'uSupport.Workspace.Ticket';
export const USUPPORT_TICKET_TYPE_WORKSPACE_ALIAS = 'uSupport.Workspace.TicketType';
export const USUPPORT_TICKET_STATUS_WORKSPACE_ALIAS = 'uSupport.Workspace.TicketStatus';
export const USUPPORT_SETTINGS_WORKSPACE_ALIAS = 'uSupport.Workspace.Settings';

/** Absolute backoffice path to an entity's workspace, e.g. workspacePath('usupport-ticket', 'edit/123'). */
export const workspacePath = (entityType: string, path = '') =>
	`section/${USUPPORT_SECTION_PATHNAME}/workspace/${entityType}/${path}`;

/** Umbraco's built-in "Richtext editor" data type (Tiptap), used for summaries and comments. */
export const USUPPORT_RICH_TEXT_DATA_TYPE_KEY = 'ca90c950-0aff-4e72-b976-a30b1ac57dad';

/** Pulls the HTML out of a serialized rich text value ({ markup, blocks }). */
export const markupOf = (json?: string | null): string => {
	try {
		return (json && JSON.parse(json)?.markup) || '';
	} catch {
		return '';
	}
};

export const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

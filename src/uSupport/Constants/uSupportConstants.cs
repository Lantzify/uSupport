using System;

namespace uSupport.Constants
{
	public static class uSupportConstants
	{
		//Section
		public const string SectionAlias = "uSupport";

		public const string uSupportSectionAccess = "uSupportSectionAccess";

		//Tree
		public const string TicketsTreeAlias = "tickets";
		public const string SettingsTreeAlias = "settings";
		public const string TicketTypesTreeAlias = "ticketTypes";
		public const string TicketStatusesTreeAlias = "ticketStatuses";

		public const string TreeGroupAlias = "uSupportGroup";

		//Table
		public const string TicketTableAlias = "uSupportTicket";
		public const string TicketTypeTableAlias = "uSupportTicketType";
		public const string TicketStatusTableAlias = "uSupportTicketStatus";
		public const string TicketCommentTableAlias = "uSupportTicketComment";

		//Default ticket types
		public static readonly Guid PageErrorTicketTypeGuid = new Guid("fc9b64b7-fa81-4de4-ae1a-8a677d314e69");
		public static readonly Guid SystemErrorTicketTypeGuid = new Guid("042839ca-e058-44ea-b6c9-2d5b337a84ef");
		public static readonly Guid GeneralQuestionTicketTypeGuid = new Guid("daf98704-3e06-4c9b-b44d-22dd8c1e494c");

		//Default ticket statuses
		public static readonly Guid NewTicketStatusGuid = new Guid("0fa7b428-efad-4617-9253-0ac162d9c672");
		public static readonly Guid InProgressTicketStatusGuid = new Guid("9b7b466c-7f61-46b1-bb1b-28986e295031");
		public static readonly Guid AnsweredTicketStatusGuid = new Guid("286b7497-bfef-449b-821f-cf6bd4bb35a3");
		public static readonly Guid ResolvedTicketStatusGuid = new Guid("23289f79-9cc2-42a0-8f27-100e2c189268");

		//Page size
		public const int PageSize = 10;

		//Cache keys
		public const string uSupportActivePagedTicketCacheKey = "uSupportPagedActiveTickets";
		public const string uSupportResolvedPagedTicketCacheKey = "uSupportPagedResolvedTickets";
	}
}
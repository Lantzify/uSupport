using NPoco;
using uSupport.Dtos.Tables;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Extensions
{
	public static class SqlExtensions
	{
		public static Sql GetFullTicket(this Sql sql)
		{
			return sql.LeftJoin(TicketTypeTableAlias)
				.On($"TypeId = UPPER({TicketTypeTableAlias}.Id)")
				.InnerJoin(TicketStatusTableAlias)
				.On($"StatusId = UPPER({TicketStatusTableAlias}.Id)");
		}

		public static string ConvertStatusesToSql(this IEnumerable<uSupportTicketStatus> statuses)
		{
			if (statuses == null) return string.Empty;

			return string.Join(", ", statuses.Select(x => string.Format("UPPER('{0}')", x.Id)));
		}

		public static string ConvertGuidToSqlString(this List<Guid> guids)
		{
			if (guids == null) return string.Empty;

			return string.Join(", ", guids.Select(x => string.Format("UPPER('{0}')", x))); ;
		}
	}
}
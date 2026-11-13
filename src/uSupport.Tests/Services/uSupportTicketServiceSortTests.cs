using uSupport.Dtos;
using uSupport.Services;

namespace uSupport.Tests.Services
{
	public class uSupportTicketServiceSortTests
	{
		[Test]
		public void NoSort_UsesDefaultColumnDescending()
			=> Assert.That(uSupportTicketService.GetOrderBy(null, "LastUpdated"), Is.EqualTo("uSupportTicket.LastUpdated DESC"));

		[Test]
		public void WhitelistedColumn_IsUsedWithDirection()
			=> Assert.That(uSupportTicketService.GetOrderBy(new uSupportSort { ColumnName = "title", Descending = false }, "LastUpdated"),
				Is.EqualTo("uSupportTicket.Title ASC"));

		[Test]
		public void UnknownColumn_FallsBackToDefault()
			=> Assert.That(uSupportTicketService.GetOrderBy(new uSupportSort { ColumnName = "Title; DROP TABLE uSupportTicket--" }, "Resolved"),
				Is.EqualTo("uSupportTicket.Resolved DESC"));
	}
}

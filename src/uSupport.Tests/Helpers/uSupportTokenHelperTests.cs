using uSupport.Helpers;
using uSupport.Dtos.Tables;

namespace uSupport.Tests.Helpers
{
	[TestFixture]
	public class uSupportTokenHelperTests
	{
		private uSupportTicket _testTicket;

		[SetUp]
		public void Setup()
		{
			_testTicket = new uSupportTicket
			{
				Id = Guid.NewGuid(),
				ExternalTicketId = "ABC-1234-567",
				Title = "Test Ticket Title",
				Summary = "This is a test ticket summary",
				AuthorId = 1,
				Submitted = new DateTime(2026, 1, 15, 10, 30, 0),
				Resolved = null,
				LastUpdated = new DateTime(2026, 1, 16, 14, 20, 0),
				LastUpdatedBy = "Admin User",
				PropertyValue = "TestValue",
				InternalComment = "Internal test comment",
				Status = new uSupportTicketStatus
				{
					Id = Guid.NewGuid(),
					Name = "Open",
					Alias = "open",
					Color = "green",
					Icon = "icon-check",
					Active = true,
					Default = true,
					Order = 1
				},
				Type = new uSupportTicketType
				{
					Id = Guid.NewGuid(),
					Name = "Bug Report",
					Alias = "bug-report",
					Color = "red",
					Icon = "icon-bug",
					Description = "Report a bug",
					Order = 1,
					PropertyId = 0,
					PropertyName = "Url",
					PropertyDescription = "Page URL where bug occurred",
					PropertyView = string.Empty
				}
			};
		}

		[Test]
		public void ReplaceTokens_WithSimpleProperty_ReplacesCorrectly()
		{
			string template = "Ticket ID: {ExternalTicketId}";
			
			var result = uSupportTokenHelper.ReplaceTokens(template, _testTicket);
			
			Assert.That(result, Is.EqualTo("Ticket ID: ABC-1234-567"));
		}

		[Test]
		public void ReplaceTokens_WithMultipleProperties_ReplacesAll()
		{
			string template = "New ticket '{ExternalTicketId}' - '{Title}' has been created";
			
			var result = uSupportTokenHelper.ReplaceTokens(template, _testTicket);
			
			Assert.That(result, Is.EqualTo("New ticket 'ABC-1234-567' - 'Test Ticket Title' has been created"));
		}

		[Test]
		public void ReplaceTokens_WithNestedProperty_ReplacesCorrectly()
		{
			string template = "Ticket {ExternalTicketId} | Status: {Status.Name}";
			
			var result = uSupportTokenHelper.ReplaceTokens(template, _testTicket);
			
			Assert.That(result, Is.EqualTo("Ticket ABC-1234-567 | Status: Open"));
		}

		[Test]
		public void ReplaceTokens_WithNonExistentProperty_ReturnsEmptyString()
		{
			string template = "Value: {NonExistentProperty}";
			
			var result = uSupportTokenHelper.ReplaceTokens(template, _testTicket);
			
			Assert.That(result, Is.EqualTo("Value: "));
		}

		[Test]
		public void ReplaceTokens_WithNullTicket_ReturnsOriginalTemplate()
		{
			string template = "Ticket: {ExternalTicketId}";

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			var result = uSupportTokenHelper.ReplaceTokens(template, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

			Assert.That(result, Is.EqualTo(template));
		}

		[Test]
		public void ReplaceTokens_WithEmptyTemplate_ReturnsEmptyString()
		{
			var result = uSupportTokenHelper.ReplaceTokens("", _testTicket);
			
			Assert.That(result, Is.EqualTo(""));
		}

		[Test]
		public void ReplaceTokens_WithNullNestedProperty_ReturnsEmptyString()
		{
			_testTicket.Status = null;
			string template = "Status: {Status.Name}";
			
			var result = uSupportTokenHelper.ReplaceTokens(template, _testTicket);
			
			Assert.That(result, Is.EqualTo("Status: "));
		}
	}
}
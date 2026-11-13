namespace uSupport.Dtos
{
	public class CreateTicketRequest
	{
		public required string Title { get; set; }
		public required string Summary { get; set; }
		public Guid TypeId { get; set; }
		public string? PropertyValue { get; set; }
	}

	public class UpdateTicketRequest
	{
		public Guid TypeId { get; set; }
		public Guid StatusId { get; set; }
		public string? InternalComment { get; set; }
		public bool SendEmail { get; set; }
	}

	public class CreateCommentRequest
	{
		public Guid TicketId { get; set; }
		public required string Comment { get; set; }
	}
}

using System;

namespace uSupport.Dtos
{
	public class PreviewDeleteItemDto
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
		public string? Type { get; set; }
		public string? Path { get; set; }
	}
}
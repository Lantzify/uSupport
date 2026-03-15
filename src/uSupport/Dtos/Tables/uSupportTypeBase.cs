using System;

namespace uSupport.Dtos.Tables
{
	public class uSupportTypeBase
	{
		public Guid Id { get; set; }
		public int Order { get; set; }
		public string Alias { get; set; }
		public string Name { get; set; }
		public string? Color { get; set; }
		public string Icon { get; set; }
	}
}
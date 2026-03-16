using uSupport.Dtos.Tables;

namespace uSupport.Dtos
{
	public class SortActionDto
	{
		public string? Type { get; set; }
		public IEnumerable<uSupportTypeBase>? List { get; set; }
	}
}
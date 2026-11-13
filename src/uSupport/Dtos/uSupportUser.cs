namespace uSupport.Dtos
{
	public class uSupportUser
	{
		public int Id { get; set; }
		public Guid Key { get; set; }
		public string? Name { get; set; }
		public string? Email { get; set; }
		public string[] AvatarUrls { get; set; } = [];
	}
}

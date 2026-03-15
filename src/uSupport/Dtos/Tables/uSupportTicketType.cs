namespace uSupport.Dtos.Tables 
{ 
	public class uSupportTicketType : uSupportTypeBase
	{
		public string? Description { get; set; }
        public int PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyDescription { get; set; }
        public string? PropertyView { get; set; }
    }
}
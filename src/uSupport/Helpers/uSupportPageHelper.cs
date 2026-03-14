using uSupport.Dtos;

namespace uSupport.Helpers
{
	public static class uSupportPageHelper
	{
		public static uSupportPage<T> MapPageToUSupportPage<T>(List<T> items, long totalItems, long currentPage, long itemsPerPage)
		{
			uSupportPage<T> page = new uSupportPage<T>()
			{
				TotalItems = totalItems,
				ItemsPerPage = itemsPerPage,
				TotalPages = (long)Math.Ceiling((decimal)totalItems / itemsPerPage),
				CurrentPage = currentPage,
				Items = items
			};

			return page;
		}
	}
}
using uSupport.Dtos.Tables;
using System.Text.RegularExpressions;

namespace uSupport.Helpers
{
	public static class uSupportTokenHelper
	{
		public static string ReplaceTokens(string template, uSupportTicket ticket)
		{
			if (string.IsNullOrWhiteSpace(template) || ticket == null)
				return template;

			return Regex.Replace(template, @"{(\w+(?:\.\w+)*)}", match =>
			{
				string propertyPath = match.Groups[1].Value;
				object? value = GetPropertyValue(ticket, propertyPath);
				return value?.ToString() ?? string.Empty;
			});
		}

		private static object? GetPropertyValue(uSupportTicket ticket, string propertyPath)
		{
			if (ticket == null)
				return null;

			string[] properties = propertyPath.Split('.');
			object? current = ticket;

			foreach (string property in properties)
			{
				if (current == null)
					return null;

				var prop = current.GetType().GetProperty(property);
				if (prop == null)
					return null;

				current = prop.GetValue(current);
			}

			return current;
		}
	}
}

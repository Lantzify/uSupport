namespace uSupport.Helpers
{
	public static class uSupportExternalIdHelper
	{
		private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		/// <summary>
		/// Human friendly ticket id, e.g. "ABC-DEFG-123".
		/// </summary>
		public static string Generate()
		{
			// ponytail: random, not guaranteed unique; ~1.5e10 combinations is plenty for a support desk
			string letters(int count) => new(Enumerable.Range(0, count).Select(_ => Letters[Random.Shared.Next(Letters.Length)]).ToArray());
			return $"{letters(3)}-{letters(4)}-{Random.Shared.Next(100, 1000)}";
		}
	}
}

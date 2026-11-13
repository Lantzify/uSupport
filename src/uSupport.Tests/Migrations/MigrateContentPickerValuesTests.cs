using uSupport.Migrations.Updates._17._0._0;

namespace uSupport.Tests.Migrations
{
	[TestFixture]
	public class MigrateContentPickerValuesTests
	{
		private static readonly Guid Key = Guid.Parse("dcf18a51-6919-4cf8-89d1-36b94ce4d963");
		private static readonly string Expected = $"\"{Key}\"";

		private static Guid? IdToKey(int id) => id == 1234 ? Key : null;

		[TestCase("1234")]
		[TestCase("\"1234\"")]
		[TestCase("\"umb://document/dcf18a5169194cf889d136b94ce4d963\"")]
		[TestCase("umb://document/dcf18a5169194cf889d136b94ce4d963")]
		public void Converts_v13_values_to_document_key(string value)
			=> Assert.That(MigrateContentPickerValues.ToKeyJson(value, IdToKey), Is.EqualTo(Expected));

		[TestCase("\"dcf18a51-6919-4cf8-89d1-36b94ce4d963\"")] // already v17
		[TestCase("\"umb://media/dcf18a5169194cf889d136b94ce4d963\"")] // not a document
		[TestCase("9999")] // id that no longer exists
		[TestCase("not a picker value")]
		[TestCase("")]
		public void Leaves_everything_else_alone(string value)
			=> Assert.That(MigrateContentPickerValues.ToKeyJson(value, IdToKey), Is.Null);
	}
}

using CommandLine;

namespace uSupport.SchemaGenerator
{
	internal class Options
	{
		[Option('o', "outputFile", Required = false,
				HelpText = "",
				Default = "..\\..\\..\\..\\uSupport\\appsettings-schema.uSupport.json")]
		public string OutputFile { get; set; } = "..\\..\\..\\..\\uSupport\\appsettings-schema.uSupport.json";
	}
}
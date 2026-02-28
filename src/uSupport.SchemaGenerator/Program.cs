using System;
using CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace uSupport.SchemaGenerator
{
	internal class Program
	{
		public static async Task Main(string[] args)
		{
			try
			{
				await Parser.Default.ParseArguments<Options>(args)
					.WithParsedAsync(Execute);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private static async Task Execute(Options options)
		{
			var generator = new SchemaGenerator();
			var schema = generator.Generate(typeof(AppSettings));
			var json = schema.ToJson();

			var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, options.OutputFile));
			Console.WriteLine($"Generating schema at: {path}");

			var directory = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			await File.WriteAllTextAsync(path, json);
			Console.WriteLine("Schema generation complete");
		}
	}
}

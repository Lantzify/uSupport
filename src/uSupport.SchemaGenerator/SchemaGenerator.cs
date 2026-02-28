using NJsonSchema;
using System.Text.Json;
using Namotion.Reflection;
using NJsonSchema.Generation;
using System.Text.Json.Serialization;

namespace uSupport.SchemaGenerator
{
	internal class SchemaGenerator : JsonSchemaGenerator
	{
		public SchemaGenerator() : base(new AutoDictionariesSchemaGeneratorSettings())
		{ }
	}

	internal class AutoDictionariesSchemaGeneratorSettings : SystemTextJsonSchemaGeneratorSettings
	{
		public AutoDictionariesSchemaGeneratorSettings()
		{
			AlwaysAllowAdditionalObjectProperties = true;
			FlattenInheritanceHierarchy = true;
			IgnoreObsoleteProperties = true;
			ReflectionService = new ReadOnlyPropertyFilteringReflectionService();
			SerializerOptions = new JsonSerializerOptions()
			{
				Converters = { new JsonStringEnumConverter() },
				IgnoreReadOnlyProperties = true,
			};
			DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
			SchemaNameGenerator = new NamespacePrefixedSchemaNameGenerator();
			GenerateExamples = true;
		}
	}

	internal class ReadOnlyPropertyFilteringReflectionService : SystemTextJsonReflectionService
	{
		public override void GenerateProperties(JsonSchema schema, ContextualType contextualType, SystemTextJsonSchemaGeneratorSettings settings, JsonSchemaGenerator schemaGenerator, JsonSchemaResolver schemaResolver)
		{
			base.GenerateProperties(schema, contextualType, settings, schemaGenerator, schemaResolver);

			if (settings.SerializerOptions?.IgnoreReadOnlyProperties ?? false)
			{
				foreach (ContextualPropertyInfo property in contextualType.Properties)
				{
					if (property.CanWrite is false)
					{
						string propertyName = GetPropertyName(property, settings);
						schema.Properties.Remove(propertyName);
					}
				}
			}
		}
	}

	internal class NamespacePrefixedSchemaNameGenerator : DefaultSchemaNameGenerator
	{
		public override string Generate(Type type)
		{
			string typeNamespace = type.Namespace?.Replace(".", string.Empty) ?? string.Empty;
			return typeNamespace + base.Generate(type);
		}
	}
}

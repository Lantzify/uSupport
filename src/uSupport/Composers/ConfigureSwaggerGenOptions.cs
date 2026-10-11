using Asp.Versioning;
using Microsoft.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using uSupport.Constants;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Common.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace uSupport.Composers
{
	internal class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
	{
		public void Configure(SwaggerGenOptions options)
			=> options.SwaggerDoc(uSupportConstants.ApiName, new OpenApiInfo { Title = "uSupport Management Api", Version = "Latest" });
	}

	/// <summary>
	/// Gives uSupport endpoints Umbraco-style operation ids (e.g. GetUsupportTicketById) without touching other APIs.
	/// </summary>
	internal class uSupportOperationIdHandler : OperationIdHandler
	{
		public uSupportOperationIdHandler(IOptions<ApiVersioningOptions> apiVersioningOptions) : base(apiVersioningOptions) { }

		protected override bool CanHandle(ApiDescription apiDescription, ControllerActionDescriptor controllerActionDescriptor)
			=> controllerActionDescriptor.ControllerTypeInfo.Namespace?.StartsWith("uSupport") is true;
	}

	/// <summary>
	/// Umbraco-style schema ids for uSupport models, so generic pages (uSupportPage<T>) get unique names.
	/// </summary>
	internal class uSupportSchemaIdHandler : SchemaIdHandler
	{
		public override bool CanHandle(Type type) => type.Namespace?.StartsWith("uSupport") is true;

		public override string Handle(Type type) => UmbracoSchemaId(type);
	}
}

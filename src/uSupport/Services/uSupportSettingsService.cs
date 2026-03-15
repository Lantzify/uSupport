using NPoco;
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using uSupport.Migrations.Schemas;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Core.Models.Email;
using Microsoft.AspNetCore.Mvc.Razor;
using uSupport.Notifications.Tickets;
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Cms.Infrastructure.Scoping;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Umbraco.Cms.Core.Configuration.Models;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportSettingsService : uSupportServiceBase<uSupportSettings, uSupportSettingsSchema>, IuSupportSettingsService
	{
		private readonly ILogger<IuSupportTicketService> _logger;
		private readonly IEmailSender _emailSender;
		private readonly IEventAggregator _eventAggregator;
		private readonly IRazorViewEngine _razorViewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IOptions<GlobalSettings> _globalSettings;
		private readonly IHttpContextAccessor _httpContextAccessor;
		
		public uSupportSettingsService(ILogger<IuSupportTicketService> logger,
			IEmailSender emailSender,
			IScopeProvider scopeProvider,
			IScopeAccessor scopeAccessor,
			IEventAggregator eventAggregator,
			IRazorViewEngine razorViewEngine,
			ITempDataProvider tempDataProvider,
			IOptions<GlobalSettings> globalSettings,
			IHttpContextAccessor httpContextAccessor) : base(SettingsTableAlias, scopeProvider, scopeAccessor)
		{
			_logger = logger;
			_emailSender = emailSender;
			_eventAggregator = eventAggregator;
			_razorViewEngine = razorViewEngine;
			_tempDataProvider = tempDataProvider;
			_globalSettings = globalSettings;
			_httpContextAccessor = httpContextAccessor;
		}

		public uSupportSettings GetSettings()
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
								.Select("*")
								.From(SettingsTableAlias);

				return context.Scope.Database.Fetch<uSupportSettings>(sql).FirstOrDefault();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public async void SendEmail(string toAddress, string subject, string templateViewPath, object model)
		{
			try
			{
                var smtpSettings = _globalSettings.Value.Smtp;

                if (string.IsNullOrEmpty(smtpSettings?.From))
                    throw new Exception("Failed to send email. Smtp from is not set in appsettings.");

                if (string.IsNullOrEmpty(toAddress) || toAddress == "None")
                    throw new Exception("Failed to send email. TicketUpdateEmail is not set in appsettings.");

				EmailMessage message;
				var emailBody = await RenderEmailTemplateAsync(templateViewPath, model);

				if (toAddress.Contains(","))
				{

					message = new(smtpSettings?.From,
												toAddress.Split(",", StringSplitOptions.RemoveEmptyEntries)
														.Select(email => email.Trim())
														.ToArray(),
												null,
												null,
												null,
												subject,
												emailBody,
												true,
												null);
				}
				else
				{

					message = new(smtpSettings?.From,
												toAddress.Trim(),
												subject,
												emailBody,
												true);
				}

				if (model is uSupportTicket ticket)
					_eventAggregator.Publish(new EmailSendingNotification(ticket.Id, toAddress, subject, templateViewPath));


				await _emailSender.SendAsync(message, emailType: "Contact");
            }
			catch (Exception ex)
			{
                _logger.LogError(ex, "Failed to send email");
            }
		}

		public async Task<string> RenderEmailTemplateAsync(string templateViewPath, object model)
		{
            if (string.IsNullOrEmpty(templateViewPath))
                throw new Exception("Failed to find email template.");

            if (!templateViewPath.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Template file must end with '.cshtml'");

			if(_httpContextAccessor.HttpContext == null)
				throw new Exception("HttpContext is null");

			ActionContext actionContext = new(_httpContextAccessor.HttpContext, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor());
            using (StringWriter stringWriter = new())
            {
                ViewEngineResult viewResult = _razorViewEngine.GetView(null, templateViewPath, false);

				if (!viewResult.Success)
				{
					var searchedLocations = string.Join(", ", viewResult.SearchedLocations ?? []);
					throw new Exception($"Failed to find template at '{templateViewPath}'. Searched locations: {searchedLocations}");
				}

				ViewDataDictionary viewData = new(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };
                ViewContext viewContext = new(actionContext,
                                                viewResult.View,
                                                viewData,
                                                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                                                stringWriter,
                                                new HtmlHelperOptions());

                await viewResult.View.RenderAsync(viewContext);

                return stringWriter.ToString();
            }
        }
	}
}

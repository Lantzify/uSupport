using uSupport.Helpers;
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Mail;
using uSupport.Dtos.Settings;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Core.Models.Email;
using Microsoft.AspNetCore.Mvc.Razor;
using uSupport.Notifications.Tickets;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Umbraco.Cms.Core.Configuration.Models;

namespace uSupport.Services
{
	public class uSupportSettingsService : IuSupportSettingsService
	{
		private const string LegacyNewTicketTemplatePath = "/App_Plugins/uSupport/templates/NewTicketEmail.cshtml";
		private const string LegacyUpdateTicketTemplatePath = "/App_Plugins/uSupport/templates/UpdateTicketEmail.cshtml";
		private const string DefaultNewTicketTemplatePath = "/Views/uSupport/Emails/NewTicketEmail.cshtml";
		private const string DefaultUpdateTicketTemplatePath = "/Views/uSupport/Emails/UpdateTicketEmail.cshtml";

		private readonly uSupportSettingsTicket _defaultSettings;
		private readonly ILogger<IuSupportTicketService> _logger;
		private readonly IEmailSender _emailSender;
		private readonly IEventAggregator _eventAggregator;
		private readonly IRazorViewEngine _razorViewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IOptions<GlobalSettings> _globalSettings;
		private readonly IHttpContextAccessor _httpContextAccessor;
		
		private readonly IOptions<uSupportSettings> _uSupportSettings;
		
        
		public uSupportSettingsService(ILogger<IuSupportTicketService> logger,
			IEmailSender emailSender,
			IEventAggregator eventAggregator,
			IRazorViewEngine razorViewEngine,
			ITempDataProvider tempDataProvider,
			IOptions<GlobalSettings> globalSettings,
			IHttpContextAccessor httpContextAccessor,
			IOptions<uSupportSettings> uSupportSettings)
        {
			_logger = logger;
			_emailSender = emailSender;
			_eventAggregator = eventAggregator;
			_razorViewEngine = razorViewEngine;
			_tempDataProvider = tempDataProvider;
			_globalSettings = globalSettings;
			_httpContextAccessor = httpContextAccessor;
			_uSupportSettings = uSupportSettings;
			
            _defaultSettings = new uSupportSettingsTicket();
		}

		public bool GetSendEmailOnTicketCreatedSetting() => _uSupportSettings.Value.Tickets.SendEmailOnTicketCreated;
		public bool GetSendEmailOnTicketCommentSetting() => _uSupportSettings.Value.Tickets.SendEmailOnTicketComment;
        public string GetTicketUpdateEmailSetting() => _uSupportSettings.Value.Tickets.TicketUpdateEmail;

		public string GetEmailSubjectNewTicket(uSupportTicket? ticket = null)
		{
			var emailSubjectNewTicket = _uSupportSettings.Value.Tickets.EmailSubjectNewTicket;
			if (!string.IsNullOrWhiteSpace(emailSubjectNewTicket))
				return uSupportTokenHelper.ReplaceTokens(emailSubjectNewTicket, ticket);

			return uSupportTokenHelper.ReplaceTokens(_defaultSettings.EmailSubjectNewTicket, ticket);
		}

		public string GetEmailSubjectUpdateTicket(uSupportTicket? ticket = null)
		{
			var emailSubjectUpdateTicket = _uSupportSettings.Value.Tickets.EmailSubjectUpdateTicket;
			if (!string.IsNullOrWhiteSpace(emailSubjectUpdateTicket))
				return uSupportTokenHelper.ReplaceTokens(emailSubjectUpdateTicket, ticket);

			return uSupportTokenHelper.ReplaceTokens(_defaultSettings.EmailSubjectUpdateTicket, ticket);
		}

		public string GetEmailTemplateNewTicketPath()
		{
			var emailTemplateNewTicketPath = _uSupportSettings.Value.Tickets.EmailTemplateNewTicketPath;
			if (!string.IsNullOrWhiteSpace(emailTemplateNewTicketPath))
				return emailTemplateNewTicketPath;

			return _defaultSettings.EmailTemplateNewTicketPath;
		}

		public string GetEmailTemplateUpdateTicketPath()
		{
			var emailTemplateUpdateTicketPath = _uSupportSettings.Value.Tickets.EmailTemplateUpdateTicketPath;
			if (!string.IsNullOrWhiteSpace(emailTemplateUpdateTicketPath))
				return emailTemplateUpdateTicketPath;

			return _defaultSettings.EmailTemplateUpdateTicketPath;
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

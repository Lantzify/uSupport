using Umbraco.Cms.Core.Mail;
using uSupport.Dtos.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Core.Models.Email;
using Microsoft.AspNetCore.Mvc.Razor;
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
		private readonly uSupportSettingsTicket _defaultSettings;
		private readonly IEmailSender _emailSender;
		private readonly IRazorViewEngine _razorViewEngine;
		private readonly IOptions<GlobalSettings> _globalSettings;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IOptions<uSupportSettings> _uSupportSettings;
		private readonly ITempDataProvider _tempDataProvider;
        private readonly ILogger<IuSupportTicketService> _logger;
		public uSupportSettingsService(IEmailSender emailSender,
			ITempDataProvider tempDataProvider,
			IRazorViewEngine razorViewEngine,
			ILogger<IuSupportTicketService> logger,
			IHostEnvironment hostingEnvironment,
			IHttpContextAccessor httpContextAccessor,
			IOptions<GlobalSettings> globalSettings,
			IOptions<uSupportSettings> uSupportSettings)
        {
			_tempDataProvider = tempDataProvider;
			_globalSettings = globalSettings;
			_uSupportSettings = uSupportSettings;
			_httpContextAccessor = httpContextAccessor;
			_razorViewEngine = razorViewEngine;
			_emailSender = emailSender;
			_logger = logger;
			
            _defaultSettings = new uSupportSettingsTicket();
		}

		public bool GetSendEmailOnTicketCreatedSetting()
		{
            return _uSupportSettings.Value.Tickets.SendEmailOnTicketCreated;
        }


        public string GetTicketUpdateEmailSetting()
		{
			return _uSupportSettings.Value.Tickets.TicketUpdateEmail;
		}

		public string GetEmailSubjectNewTicket()
		{
			var emailSubjectNewTicket = _uSupportSettings.Value.Tickets.EmailSubjectNewTicket;
			if (!string.IsNullOrWhiteSpace(emailSubjectNewTicket))
				return emailSubjectNewTicket;

			return _defaultSettings.EmailSubjectNewTicket;
		}

		public string GetEmailSubjectUpdateTicket()
		{
			var emailSubjectUpdateTicket = _uSupportSettings.Value.Tickets.EmailSubjectUpdateTicket;
			if (!string.IsNullOrWhiteSpace(emailSubjectUpdateTicket))
				return emailSubjectUpdateTicket;

			return _defaultSettings.EmailSubjectUpdateTicket;
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

            if (!templateViewPath.EndsWith(".cshtml"))
                throw new Exception("Template file must end with '.cshtml'");

            ActionContext actionContext = new(_httpContextAccessor.HttpContext, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor());
            using (StringWriter stringWriter = new())
            {
                ViewEngineResult viewResult = _razorViewEngine.GetView(templateViewPath, templateViewPath, false);

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
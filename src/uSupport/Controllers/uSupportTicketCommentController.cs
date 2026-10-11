using uSupport.Dtos;
using uSupport.Helpers;
using uSupport.Dtos.Tables;
using uSupport.Notifications;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Security;
using uSupport.Migrations.Schemas;
using Microsoft.Extensions.Logging;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Api.Management.Routing;

namespace uSupport.Controllers
{
	[VersionedApiBackOfficeRoute("usupport/ticket-comment")]
	[ApiExplorerSettings(GroupName = "Ticket comment")]
	public class uSupportTicketCommentController : uSupportControllerBase
	{
		private readonly IHtmlSanitizer _htmlSanitizer;
		private readonly uSupportUserHelper _userHelper;
		private readonly IEventAggregator _eventAggregator;
		private readonly ILogger<uSupportTicketCommentController> _logger;
		private readonly IuSupportTicketService _uSupportTicketService;
		private readonly IuSupportSettingsService _uSupportSettingsService;
		private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
		private readonly IuSupportTicketCommentService _uSupportTicketCommentService;

		public uSupportTicketCommentController(IHtmlSanitizer htmlSanitizer,
			uSupportUserHelper userHelper,
			IEventAggregator eventAggregator,
			ILogger<uSupportTicketCommentController> logger,
			IuSupportTicketService uSupportTicketService,
			IuSupportSettingsService uSupportSettingsService,
			IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
			IuSupportTicketCommentService uSupportTicketCommentService)
		{
			_htmlSanitizer = htmlSanitizer;
			_userHelper = userHelper;
			_eventAggregator = eventAggregator;
			_logger = logger;
			_uSupportTicketService = uSupportTicketService;
			_uSupportSettingsService = uSupportSettingsService;
			_backOfficeSecurityAccessor = backOfficeSecurityAccessor;
			_uSupportTicketCommentService = uSupportTicketCommentService;
		}

		[HttpGet("ticket/{ticketId:guid}")]
		[ProducesResponseType<uSupportPage<uSupportTicketComment>>(StatusCodes.Status200OK)]
		public IActionResult GetByTicket(Guid ticketId, long page = 1)
			=> Ok(_uSupportTicketCommentService.GetPagedCommentsForTicket(ticketId, page));

		[HttpPost]
		[ProducesResponseType<uSupportTicketComment>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Create(CreateCommentRequest request)
		{
			var ticket = _uSupportTicketService.Get(request.TicketId);
			if (ticket == null)
				return NotFound();

			var user = CurrentUser(_backOfficeSecurityAccessor);

			var comment = _uSupportTicketCommentService.Create(new uSupportTicketCommentSchema
			{
				TicketId = ticket.Id,
				UserId = user.Id,
				Comment = _htmlSanitizer.Sanitize(request.Comment)
			});
			comment.User = _userHelper.GetUser(user.Id);

			var ticketSchema = ticket.ConvertDtoToSchema();
			ticketSchema.LastUpdatedBy = user.Name;
			ticketSchema.LastUpdated = DateTime.Now;

			var updatedTicket = _uSupportTicketService.Update(ticketSchema);
			updatedTicket.Author = _userHelper.GetUser(updatedTicket.AuthorId);
			updatedTicket.Comments = _uSupportTicketCommentService.GetCommentsFromTicketId(updatedTicket.Id);

			var settings = _uSupportSettingsService.GetSettings();
			if (settings != null && settings.SendEmailOnTicketComment)
			{
				// The author hears about other people's comments; support hears about the author's.
				var toAddress = ticket.AuthorId != user.Id
					? updatedTicket.Author?.Email
					: settings.TicketUpdateEmail;

				if (!string.IsNullOrWhiteSpace(toAddress))
				{
					await _uSupportSettingsService.SendEmail(
						toAddress,
						uSupportTokenHelper.ReplaceTokens(settings.EmailSubjectUpdateTicket, updatedTicket),
						settings.EmailTemplateUpdateTicketPath,
						updatedTicket);
				}
				else
				{
					_logger.LogWarning("Email was not sent for ticket '{TicketId}' on comment because no to address was found.", updatedTicket.ExternalTicketId);
				}
			}

			_eventAggregator.Publish(new AddTicketCommentNotification(updatedTicket, comment));

			return Ok(comment);
		}
	}
}

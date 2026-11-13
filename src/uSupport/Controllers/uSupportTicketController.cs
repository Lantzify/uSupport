using uSupport.Dtos;
using uSupport.Helpers;
using uSupport.Constants;
using uSupport.Dtos.Tables;
using uSupport.Notifications;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Security;
using uSupport.Migrations.Schemas;
using Microsoft.Extensions.Logging;
using uSupport.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Umbraco.Cms.Api.Management.Routing;

namespace uSupport.Controllers
{
	[VersionedApiBackOfficeRoute("usupport/ticket")]
	[ApiExplorerSettings(GroupName = "Ticket")]
	public class uSupportTicketController : uSupportControllerBase
	{
		private readonly IHtmlSanitizer _htmlSanitizer;
		private readonly uSupportUserHelper _userHelper;
		private readonly IEventAggregator _eventAggregator;
		private readonly ILogger<uSupportTicketController> _logger;
		private readonly IuSupportTicketService _uSupportTicketService;
		private readonly IuSupportSettingsService _uSupportSettingsService;
		private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;
		private readonly IuSupportTicketCommentService _uSupportTicketCommentService;
		private readonly IuSupportTicketHistoryService _uSupportTicketHistoryService;

		public uSupportTicketController(IHtmlSanitizer htmlSanitizer,
			uSupportUserHelper userHelper,
			IEventAggregator eventAggregator,
			ILogger<uSupportTicketController> logger,
			IuSupportTicketService uSupportTicketService,
			IuSupportSettingsService uSupportSettingsService,
			IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
			IuSupportTicketStatusService uSupportTicketStatusService,
			IuSupportTicketCommentService uSupportTicketCommentService,
			IuSupportTicketHistoryService uSupportTicketHistoryService)
		{
			_htmlSanitizer = htmlSanitizer;
			_userHelper = userHelper;
			_eventAggregator = eventAggregator;
			_logger = logger;
			_uSupportTicketService = uSupportTicketService;
			_uSupportSettingsService = uSupportSettingsService;
			_backOfficeSecurityAccessor = backOfficeSecurityAccessor;
			_uSupportTicketStatusService = uSupportTicketStatusService;
			_uSupportTicketCommentService = uSupportTicketCommentService;
			_uSupportTicketHistoryService = uSupportTicketHistoryService;
		}

		[HttpGet("active")]
		[ProducesResponseType<uSupportPage<uSupportTicket>>(StatusCodes.Status200OK)]
		public IActionResult GetActive(long page = 1, string? searchTerm = null, string? sortColumn = null, bool sortDescending = true)
			=> Ok(_uSupportTicketService.GetPagedActiveTickets(page, searchTerm, ToSort(sortColumn, sortDescending)));

		[HttpGet("resolved")]
		[ProducesResponseType<uSupportPage<uSupportTicket>>(StatusCodes.Status200OK)]
		public IActionResult GetResolved(long page = 1, string? searchTerm = null, string? sortColumn = null, bool sortDescending = true)
			=> Ok(_uSupportTicketService.GetPagedResolvedTickets(page, searchTerm, ToSort(sortColumn, sortDescending)));

		[HttpGet("any-resolved")]
		[ProducesResponseType<bool>(StatusCodes.Status200OK)]
		public IActionResult AnyResolved() => Ok(_uSupportTicketService.AnyResolvedTickets());

		[HttpGet("{id:guid}")]
		[ProducesResponseType<uSupportTicket>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetById(Guid id)
		{
			var ticket = _uSupportTicketService.Get(id);
			if (ticket == null)
				return NotFound();

			ticket.Author = _userHelper.GetUser(ticket.AuthorId);
			return Ok(ticket);
		}

		[HttpGet("{id:guid}/history")]
		[ProducesResponseType<uSupportPage<uSupportTicketHistory>>(StatusCodes.Status200OK)]
		public IActionResult GetHistory(Guid id, long page = 1) => Ok(_uSupportTicketHistoryService.GetPagedByTicketId(id, page));

		[HttpPost]
		[ProducesResponseType<uSupportTicket>(StatusCodes.Status200OK)]
		public async Task<IActionResult> Create(CreateTicketRequest request)
		{
			var author = CurrentUser(_backOfficeSecurityAccessor);

			var ticket = _uSupportTicketService.Create(new uSupportTicketSchema
			{
				Title = request.Title,
				Summary = _htmlSanitizer.Sanitize(request.Summary),
				TypeId = request.TypeId,
				PropertyValue = request.PropertyValue,
				StatusId = _uSupportTicketStatusService.GetDefaultStatus().Id,
				AuthorId = author.Id,
				LastUpdatedBy = author.Name,
				ExternalTicketId = uSupportExternalIdHelper.Generate()
			});
			ticket.Author = _userHelper.GetUser(author.Id);

			var settings = _uSupportSettingsService.GetSettings();
			if (settings != null && settings.SendEmailOnTicketCreated)
			{
				if (!string.IsNullOrWhiteSpace(settings.TicketUpdateEmail))
				{
					await _uSupportSettingsService.SendEmail(
						settings.TicketUpdateEmail,
						uSupportTokenHelper.ReplaceTokens(settings.EmailSubjectNewTicket, ticket),
						settings.EmailTemplateNewTicketPath,
						ticket);
				}
				else
				{
					_logger.LogWarning("Email for ticket {ExternalTicketId} was not sent due to 'TicketUpdateEmail' not being set", ticket.ExternalTicketId);
				}
			}

			return Ok(ticket);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]
		[ProducesResponseType<uSupportTicket>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(Guid id, UpdateTicketRequest request)
		{
			var oldTicket = _uSupportTicketService.Get(id);
			if (oldTicket == null)
				return NotFound();

			var user = CurrentUser(_backOfficeSecurityAccessor);
			var ticket = oldTicket.ConvertDtoToSchema();

			if (oldTicket.StatusId != request.StatusId)
			{
				var newStatus = _uSupportTicketStatusService.Get(request.StatusId);
				ticket.StatusId = request.StatusId;
				ticket.Resolved = newStatus.Active ? null : DateTime.Now;
			}

			if (oldTicket.TypeId != request.TypeId)
			{
				ticket.TypeId = request.TypeId;
				ticket.PropertyValue = string.Empty;
			}

			ticket.InternalComment = request.InternalComment;
			ticket.LastUpdatedBy = user.Name;
			ticket.LastUpdated = DateTime.Now;

			var updatedTicket = _uSupportTicketService.Update(ticket);
			updatedTicket.Author = _userHelper.GetUser(updatedTicket.AuthorId);

			if (request.SendEmail)
			{
				var settings = _uSupportSettingsService.GetSettings();
				if (settings != null && !string.IsNullOrWhiteSpace(updatedTicket.Author?.Email))
				{
					updatedTicket.Comments = _uSupportTicketCommentService.GetCommentsFromTicketId(id);

					await _uSupportSettingsService.SendEmail(
						updatedTicket.Author.Email,
						uSupportTokenHelper.ReplaceTokens(settings.EmailSubjectUpdateTicket, updatedTicket),
						settings.EmailTemplateUpdateTicketPath,
						updatedTicket);

					_eventAggregator.Publish(new UpdateTicketSendEmailNotification(updatedTicket));
				}
				else
				{
					_logger.LogWarning("Email was not sent for ticket '{TicketId}' on update because uSupport settings or the author's email are missing.", updatedTicket.ExternalTicketId);
				}
			}

			if (!updatedTicket.Status?.Active ?? false)
				_eventAggregator.Publish(new UpdateTicketResolvedNotification(updatedTicket));

			_eventAggregator.Publish(new TicketHistoryNotification(user.Id, oldTicket, updatedTicket));

			return Ok(updatedTicket);
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult Delete(Guid id)
		{
			if (_uSupportTicketService.Get(id) == null)
				return NotFound();

			_uSupportTicketService.Delete(id);
			return Ok();
		}

		private static uSupportSort? ToSort(string? sortColumn, bool sortDescending)
			=> string.IsNullOrWhiteSpace(sortColumn) ? null : new uSupportSort { ColumnName = sortColumn, Descending = sortDescending };
	}
}

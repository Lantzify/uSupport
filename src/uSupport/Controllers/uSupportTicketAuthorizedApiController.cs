using uSupport.Dtos;
using uSupport.Dtos.Tables;
using uSupport.Notifications;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Services;
using uSupport.Migrations.Schemas;
using Microsoft.Extensions.Logging;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Core.Models.ContentEditing;
using static uSupport.Helpers.uSupportTypeHelper;

namespace uSupport.Controllers
{
	public class uSupportTicketAuthorizedApiController : UmbracoAuthorizedApiController
	{
		private readonly IUserService _userService;
        private readonly IUmbracoMapper _umbracoMapper;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<IuSupportTicketService> _logger;
        private readonly IuSupportTicketService _uSupportTicketService;
		private readonly IuSupportSettingsService _uSupportSettingsService;
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;
		private readonly IuSupportTicketCommentService _uSupportTicketCommentService;
		private readonly IuSupportTicketHistoryService _uSupportTicketHistoryService;


		public uSupportTicketAuthorizedApiController(IUserService userService,
            IUmbracoMapper umbracoMapper,
			IEventAggregator eventAggregator,
			ILogger<IuSupportTicketService> logger,
			IuSupportTicketService uSupportTicketService,
			IuSupportTicketStatusService uSupportTicketStatusService,
			IuSupportSettingsService uSupportSettingsService,
			IuSupportTicketCommentService uSupportTicketCommentService,
			IuSupportTicketHistoryService uSupportTicketHistoryService)
		{
			_eventAggregator = eventAggregator;
			_logger = logger;
			_umbracoMapper = umbracoMapper;
			_userService = userService;
			_uSupportTicketService = uSupportTicketService;
			_uSupportTicketStatusService = uSupportTicketStatusService;
			_uSupportSettingsService = uSupportSettingsService;
			_uSupportTicketCommentService = uSupportTicketCommentService;
			_uSupportTicketHistoryService = uSupportTicketHistoryService;
		}

		[HttpGet]
		public uSupportPage<uSupportTicket> GetPagedActiveTickets(long page, string? searchTerm = null, string? sortColumnName = null, bool? sortReverse = null)
		{
			return _uSupportTicketService.GetPagedActiveTickets(page, searchTerm, !string.IsNullOrWhiteSpace(sortColumnName) ? 
				new uSupportSort
				{
					ColumnName = sortColumnName,
					Reverse = sortReverse ?? false
				} : null
			);
		} 

		[HttpGet]
		public uSupportPage<uSupportTicket> GetPagedResolvedTickets(long page, string? searchTerm = null, string? sortColumnName = null, bool? sortReverse = null)
		{
			return _uSupportTicketService.GetPagedResolvedTickets(page, searchTerm, !string.IsNullOrWhiteSpace(sortColumnName) ?
				new uSupportSort
				{
					ColumnName = sortColumnName,
					Reverse = sortReverse ?? false
				} : null
			);
		}

		[HttpGet]
		public IEnumerable<uSupportTicketHistory> GetTicketHistory(Guid ticketId)
		{
			return _uSupportTicketHistoryService.GetByTicketId(ticketId);
		}


		[HttpGet]
		public bool AnyResolvedTickets() => _uSupportTicketService.AnyResolvedTickets();

		[HttpGet]
		public uSupportTicket GetTicket(Guid ticketId)
		{
			var ticket = _uSupportTicketService.Get(ticketId);
			var author = _userService.GetUserById(ticket.AuthorId);

            ticket.Author = _umbracoMapper.Map<IUser, UserDisplay>(author);
            ticket.Comments = _uSupportTicketCommentService.GetCommentsFromTicketId(ticketId);

			return ticket;
        } 

		[HttpPost]
		public ActionResult<uSupportTicket> CreateTicket(uSupportTicketSchema ticket)
		{
			try
			{
				ticket.StatusId = _uSupportTicketStatusService.GetDefaultStatus().Id;
				var createdTicket = _uSupportTicketService.Create(ticket);

                var author = _userService.GetUserById(ticket.AuthorId);
                createdTicket.Author = _umbracoMapper.Map<IUser, UserDisplay>(author);

				if (_uSupportSettingsService.GetSendEmailOnTicketCreatedSetting())
				{
                    _uSupportSettingsService.SendEmail(
                        _uSupportSettingsService.GetTicketUpdateEmailSetting(),
						_uSupportSettingsService.GetEmailSubjectNewTicket(createdTicket),
                        _uSupportSettingsService.GetEmailTemplateNewTicketPath(),
                        createdTicket);
                }				

				return createdTicket;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create ticket");
				return ValidationProblem("Failed to create ticket");
			}
		}

		[HttpPost]
		public ActionResult<uSupportTicket> UpdateTicket(UpdateTicketDto dto)
		{
            try
			{
				var oldTicket = _uSupportTicketService.Get(dto.Ticket.Id);

				var oldStatus = _uSupportTicketStatusService.Get(oldTicket.StatusId);
				if (oldStatus.Id != dto.Ticket.StatusId)
				{
					var newStatus = _uSupportTicketStatusService.Get(dto.Ticket.StatusId);
					if (newStatus.Active)
						dto.Ticket.Resolved = default;
					else
						dto.Ticket.Resolved = DateTime.Now;
				}

				if (oldTicket.TypeId != dto.Ticket.TypeId)
					dto.Ticket.PropertyValue = null;

				var updatedTicket = _uSupportTicketService.Update(dto.Ticket.ConvertDtoToSchema());
				
				if (dto.SendEmail)
				{
					_uSupportSettingsService.SendEmail(
						_userService.GetUserById(dto.Ticket.AuthorId).Email,
						_uSupportSettingsService.GetEmailSubjectUpdateTicket(updatedTicket),
						_uSupportSettingsService.GetEmailTemplateUpdateTicketPath(),
						updatedTicket);

					_eventAggregator.Publish(new UpdateTicketSendEmailNotification(updatedTicket));
				}


				if (!updatedTicket.Status.Active)			
                    _eventAggregator.Publish(new UpdateTicketResolvedNotification(updatedTicket));

				_eventAggregator.Publish(new TicketHistoryNotification(dto.UserId, oldTicket, updatedTicket));
               
                return updatedTicket;
			}
            catch (Exception ex)
            {
				_logger.LogError(ex, "Failed to update ticket '{TicketId}'", dto.Ticket.ExternalTicketId);
				return ValidationProblem("Failed to update ticket");
			}
		}

		[HttpGet]
		public void DeleteTicket(Guid ticketId)
		{
            _uSupportTicketService.Delete(ticketId);
        }
	}
}
using uSupport.Dtos;
using Umbraco.Extensions;
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Cache;
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
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Controllers
{
	public class uSupportTicketAuthorizedApiController : UmbracoAuthorizedApiController
	{
		private readonly IUserService _userService;
		private readonly AppCaches _appCaches;
        private readonly IUmbracoMapper _umbracoMapper;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<IuSupportTicketService> _logger;
        private readonly IuSupportTicketService _uSupportTicketService;
		private readonly IuSupportSettingsService _uSupportSettingsService;
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;
		private readonly IuSupportTicketCommentService _uSupportTicketCommentService;

		public uSupportTicketAuthorizedApiController(IUserService userService,
			AppCaches appCaches,
            IUmbracoMapper umbracoMapper,
            IEventAggregator eventAggregator,
            ILogger<IuSupportTicketService> logger,
            IuSupportTicketService uSupportTicketService,
			IuSupportTicketStatusService uSupportTicketStatusService,
			IuSupportSettingsService uSupportSettingsService,
			IuSupportTicketCommentService uSupportTicketCommentService)
		{
			_eventAggregator = eventAggregator;
			_logger = logger;
			_appCaches = appCaches;
			_umbracoMapper = umbracoMapper;
            _userService = userService;
			_uSupportTicketService = uSupportTicketService;
			_uSupportTicketStatusService = uSupportTicketStatusService;
			_uSupportSettingsService = uSupportSettingsService;
			_uSupportTicketCommentService = uSupportTicketCommentService;
		}

		[HttpGet]
		public uSupportPage<uSupportTicket> GetPagedActiveTickets(long page)
		{
			return _appCaches.RuntimeCache.GetCacheItem(uSupportActivePagedTicketCacheKey + page, () =>
			{
				return _uSupportTicketService.GetPagedActiveTickets(page);
			});
		} 

		[HttpGet]
		public uSupportPage<uSupportTicket> GetPagedResolvedTickets(long page)
		{
            return _appCaches.RuntimeCache.GetCacheItem(uSupportResolvedPagedTicketCacheKey + page, () =>
            {
                return _uSupportTicketService.GetPagedResolvedTickets(page);
            });
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
                        _uSupportSettingsService.GetEmailSubjectNewTicket(),
                        _uSupportSettingsService.GetEmailTemplateNewTicketPath(),
                        createdTicket);
                }

				_uSupportTicketService.ClearTicketCache();

				_eventAggregator.Publish(new CreateTicketNotification(createdTicket));

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
						_uSupportSettingsService.GetEmailSubjectUpdateTicket(),
						_uSupportSettingsService.GetEmailTemplateUpdateTicketPath(),
						updatedTicket);

					_eventAggregator.Publish(new UpdateTicketSendEmailNotification(updatedTicket));
				}

                _uSupportTicketService.ClearTicketCache();

				if (!updatedTicket.Status.Active)			
                    _eventAggregator.Publish(new UpdateTicketResolvedNotification(updatedTicket));

                _eventAggregator.Publish(new UpdateTicketNotification(updatedTicket));

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
            _eventAggregator.Publish(new DeleteTicketNotification(_uSupportTicketService.Get(ticketId)));
            _uSupportTicketService.Delete(ticketId);
            _uSupportTicketService.ClearTicketCache();
        }
	}
}
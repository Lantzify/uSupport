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
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace uSupport.Controllers
{
    public class uSupportTicketCommentAuthorizedApiController : UmbracoAuthorizedApiController
    {
        private readonly ILogger<IuSupportTicketCommentService> _logger;
        private readonly IUmbracoMapper _umbracoMapper;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserService _userService;
        private readonly IuSupportTicketService _uSupportTicketService;
        private readonly IuSupportSettingsService _uSupportSettingsService;
        private readonly IuSupportTicketCommentService _uSupportTicketCommentService;


        public uSupportTicketCommentAuthorizedApiController(
            ILogger<IuSupportTicketCommentService> logger,
            IUmbracoMapper umbracoMapper,
            IEventAggregator eventAggregator,
            IUserService userService,
            IuSupportTicketService uSupportTicketService,
            IuSupportSettingsService uSupportSettingsService,
            IuSupportTicketCommentService uSupportTicketCommentService)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;
            _userService = userService;
            _umbracoMapper = umbracoMapper;
            _uSupportTicketService = uSupportTicketService;
            _uSupportSettingsService = uSupportSettingsService;
            _uSupportTicketCommentService = uSupportTicketCommentService;
        }

        [HttpGet]
        public IEnumerable<uSupportTicketComment> GetCommentsFromTicketId(Guid ticketId) => _uSupportTicketCommentService.GetCommentsFromTicketId(ticketId).ToList();


        [HttpPost]
        public ActionResult<IEnumerable<uSupportTicketComment>> Comment(uSupportTicketCommentSchema ticketComment)
        {
            try
            {
                var comment = _uSupportTicketCommentService.Create(ticketComment);

                var ticket = _uSupportTicketService.Get(ticketComment.TicketId);
                var author = _userService.GetUserById(ticket.AuthorId);
                ticket.Author = _umbracoMapper.Map<IUser, UserDisplay>(author);

                _uSupportSettingsService.SendEmail(
                    _uSupportSettingsService.GetTicketUpdateEmailSetting(),
                    _uSupportSettingsService.GetEmailSubjectNewTicket(),
                    _uSupportSettingsService.GetEmailTemplateNewTicketPath(),
                    ticket);

                _eventAggregator.Publish(new AddTicketCommentNotification(ticket, comment));

                return _uSupportTicketCommentService.GetCommentsFromTicketId(ticketComment.TicketId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return ValidationProblem("Failed to comment. (Check logs for futher information)");
            }
        }

        [HttpPost]
        public ActionResult<uSupportTicketComment> UpdateTicketComment(uSupportTicketCommentSchema ticketComment)
        {
            try
            {
                if (ticketComment == null) return ValidationProblem("No comment was found");

                return _uSupportTicketCommentService.Update(ticketComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return ValidationProblem("Failed to update comment. (Check logs for futher information)");
            }

        }

        [HttpGet]
        public void DeleteTicketComment(Guid id) => _uSupportTicketCommentService.Delete(id);
    }
}
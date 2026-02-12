using uSupport.Helpers;
using uSupport.Dtos.Tables;
using uSupport.Notifications;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using uSupport.Migrations.Schemas;
using Microsoft.Extensions.Logging;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace uSupport.Controllers
{
    public class uSupportTicketStatusAuthorizedApiController : UmbracoAuthorizedApiController
    {
        private readonly ILogger<IuSupportTicketStatusService> _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly IuSupportTicketStatusService _uSupportTicketStatusService;

        public uSupportTicketStatusAuthorizedApiController(
            ILogger<IuSupportTicketStatusService> logger,
            IEventAggregator eventAggregator,
            IuSupportTicketStatusService uSupportTicketStatusService)
        {
            _logger = logger;
            _eventAggregator = eventAggregator;
            _uSupportTicketStatusService = uSupportTicketStatusService;
        }

        [HttpGet]
        public IEnumerable<uSupportTicketStatus> GetAllTicketStatuses() => _uSupportTicketStatusService.GetAll();

        [HttpGet]
        public uSupportTicketStatus GetTicketStatus(Guid id) => _uSupportTicketStatusService.Get(id);

        [HttpPost]
        public IEnumerable<uSupportTicketStatus> GetTicketStatuses(List<Guid> ids)
        {
            if (ids == null) return null;

            return _uSupportTicketStatusService.GetByIds(ids);
        }

        [HttpGet]
        public Guid GetStatusIdFromName(string statusName) => _uSupportTicketStatusService.GetStatusIdFromName(statusName);

        [HttpPost]
        public ActionResult<uSupportTicketStatus> CreateTicketStatus(uSupportTicketStatusSchema ticketStatus)
        {
            try
            {
                ticketStatus.Order = _uSupportTicketStatusService.GetStatusCount() + 1;
                var status = _uSupportTicketStatusService.Create(ticketStatus);
                _eventAggregator.Publish(new CreateTicketStatusNotification(status));

                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create ticket status.");
                return ValidationProblem("Failed to create ticket status.");
            }
        }

        [HttpPost]
        public ActionResult<uSupportTicketStatus> UpdateTicketStatus(uSupportTicketStatus ticketStatus)
        {
            try
            {
                var status = _uSupportTicketStatusService.Update(ticketStatus.ConvertDtoToSchema());
                _eventAggregator.Publish(new UpdateTicketStatusNotification(status));
                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update ticket status '{TicketStatusName}'", ticketStatus.Name);
                return ValidationProblem("Failed to update ticket status.");
            }

        }

        [HttpGet]
        public void DeleteTicket(Guid id)
        {
            var ticketStatus = _uSupportTicketStatusService.Get(id);
            _eventAggregator.Publish(new DeleteTicketStatusNotification(ticketStatus));
            _uSupportTicketStatusService.Delete(id);
        }
            
    }
}
using uSupport.Dtos;
using uSupport.Helpers;
using uSupport.Constants;
using uSupport.Dtos.Tables;
using uSupport.Notifications;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using uSupport.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Umbraco.Cms.Api.Management.Routing;

namespace uSupport.Controllers
{
	[VersionedApiBackOfficeRoute("usupport/ticket-status")]
	[ApiExplorerSettings(GroupName = "Ticket status")]
	public class uSupportTicketStatusController : uSupportControllerBase
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IuSupportTicketService _uSupportTicketService;
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;

		public uSupportTicketStatusController(IEventAggregator eventAggregator,
			IuSupportTicketService uSupportTicketService,
			IuSupportTicketStatusService uSupportTicketStatusService)
		{
			_eventAggregator = eventAggregator;
			_uSupportTicketService = uSupportTicketService;
			_uSupportTicketStatusService = uSupportTicketStatusService;
		}

		[HttpGet]
		[ProducesResponseType<IEnumerable<uSupportTicketStatus>>(StatusCodes.Status200OK)]
		public IActionResult GetAll() => Ok(_uSupportTicketStatusService.GetAll());

		[HttpGet("{id:guid}")]
		[ProducesResponseType<uSupportTicketStatus>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetById(Guid id)
		{
			var status = _uSupportTicketStatusService.Get(id);
			return status == null ? NotFound() : Ok(status);
		}

		[HttpPost]
		[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]
		[ProducesResponseType<uSupportTicketStatus>(StatusCodes.Status200OK)]
		public IActionResult Create(uSupportTicketStatus model)
		{
			var schema = model.ConvertDtoToSchema();
			schema.Id = Guid.NewGuid();
			schema.Order = _uSupportTicketStatusService.GetStatusCount() + 1;

			var status = _uSupportTicketStatusService.Create(schema);
			_eventAggregator.Publish(new CreateTicketStatusNotification(status));

			if (status.Default)
				ClearOtherDefaults(status.Id);

			return Ok(status);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]
		[ProducesResponseType<uSupportTicketStatus>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult Update(Guid id, uSupportTicketStatus model)
		{
			var existing = _uSupportTicketStatusService.Get(id);
			if (existing == null)
				return NotFound();

			model.Id = id;
			model.Order = existing.Order;

			var status = _uSupportTicketStatusService.Update(model.ConvertDtoToSchema());
			_eventAggregator.Publish(new UpdateTicketStatusNotification(status));

			if (status.Default)
				ClearOtherDefaults(status.Id);

			return Ok(status);
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult Delete(Guid id)
		{
			var status = _uSupportTicketStatusService.Get(id);
			if (status == null)
				return NotFound();

			if (GetDependentTicketItems(id).Any())
				return BadRequest("The ticket status is used by one or more tickets.");

			_uSupportTicketStatusService.Delete(id);
			_eventAggregator.Publish(new DeleteTicketStatusNotification(status));

			return Ok();
		}

		[HttpGet("{id:guid}/dependent-tickets")]
		[ProducesResponseType<IEnumerable<PreviewDeleteItemDto>>(StatusCodes.Status200OK)]
		public IActionResult GetDependentTickets(Guid id) => Ok(GetDependentTicketItems(id));

		[HttpPut("sort")]
		[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IActionResult Sort(List<Guid> ids)
		{
			for (var i = 0; i < ids.Count; i++)
			{
				var status = _uSupportTicketStatusService.Get(ids[i]);
				if (status == null)
					continue;

				status.Order = i + 1;
				_uSupportTicketStatusService.Update(status.ConvertDtoToSchema());
				_eventAggregator.Publish(new UpdateTicketStatusNotification(status));
			}

			return Ok();
		}

		// New tickets get the default status, so there can only be one.
		private void ClearOtherDefaults(Guid defaultId)
		{
			foreach (var other in _uSupportTicketStatusService.GetAll().Where(x => x.Default && x.Id != defaultId))
			{
				other.Default = false;
				_uSupportTicketStatusService.Update(other.ConvertDtoToSchema());
				_eventAggregator.Publish(new UpdateTicketStatusNotification(other));
			}
		}

		// ponytail: loads every ticket and filters in memory; add a WHERE StatusId query if ticket counts grow large
		private IEnumerable<PreviewDeleteItemDto> GetDependentTicketItems(Guid id)
			=> _uSupportTicketService.GetAll().Where(x => x.StatusId == id).ConvertToDeletePreviewDto();
	}
}

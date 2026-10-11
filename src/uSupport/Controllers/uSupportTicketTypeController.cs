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
	[VersionedApiBackOfficeRoute("usupport/ticket-type")]
	[ApiExplorerSettings(GroupName = "Ticket type")]
	public class uSupportTicketTypeController : uSupportControllerBase
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IuSupportTicketService _uSupportTicketService;
		private readonly IuSupportTicketTypeService _uSupportTicketTypeService;

		public uSupportTicketTypeController(IEventAggregator eventAggregator,
			IuSupportTicketService uSupportTicketService,
			IuSupportTicketTypeService uSupportTicketTypeService)
		{
			_eventAggregator = eventAggregator;
			_uSupportTicketService = uSupportTicketService;
			_uSupportTicketTypeService = uSupportTicketTypeService;
		}

		[HttpGet]
		[ProducesResponseType<IEnumerable<uSupportTicketType>>(StatusCodes.Status200OK)]
		public IActionResult GetAll() => Ok(_uSupportTicketTypeService.GetAll());

		[HttpGet("{id:guid}")]
		[ProducesResponseType<uSupportTicketType>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetById(Guid id)
		{
			var type = _uSupportTicketTypeService.Get(id);
			return type == null ? NotFound() : Ok(type);
		}

		[HttpPost]
		[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]
		[ProducesResponseType<uSupportTicketType>(StatusCodes.Status200OK)]
		public IActionResult Create(uSupportTicketType model)
		{
			var schema = model.ConvertDtoToSchema();
			schema.Id = Guid.NewGuid();
			schema.Order = _uSupportTicketTypeService.GetTypesCount() + 1;

			var type = _uSupportTicketTypeService.Create(schema);
			_eventAggregator.Publish(new CreateTicketTypeNotification(type));

			return Ok(type);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]
		[ProducesResponseType<uSupportTicketType>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult Update(Guid id, uSupportTicketType model)
		{
			var existing = _uSupportTicketTypeService.Get(id);
			if (existing == null)
				return NotFound();

			model.Id = id;
			model.Order = existing.Order;

			var type = _uSupportTicketTypeService.Update(model.ConvertDtoToSchema());
			_eventAggregator.Publish(new UpdateTicketTypeNotification(type));

			return Ok(type);
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Policy = uSupportConstants.uSupportSectionAccess)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult Delete(Guid id)
		{
			var type = _uSupportTicketTypeService.Get(id);
			if (type == null)
				return NotFound();

			if (GetDependentTicketItems(id).Any())
				return BadRequest("The ticket type is used by one or more tickets.");

			_uSupportTicketTypeService.Delete(id);
			_eventAggregator.Publish(new DeleteTicketTypeNotification(type));

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
				var type = _uSupportTicketTypeService.Get(ids[i]);
				if (type == null)
					continue;

				type.Order = i + 1;
				_uSupportTicketTypeService.Update(type.ConvertDtoToSchema());
				_eventAggregator.Publish(new UpdateTicketTypeNotification(type));
			}

			return Ok();
		}

		// ponytail: loads every ticket and filters in memory; add a WHERE TypeId query if ticket counts grow large
		private IEnumerable<PreviewDeleteItemDto> GetDependentTicketItems(Guid id)
			=> _uSupportTicketService.GetAll().Where(x => x.TypeId == id).ConvertToDeletePreviewDto();
	}
}

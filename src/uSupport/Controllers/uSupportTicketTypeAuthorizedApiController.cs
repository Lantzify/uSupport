using uSupport.Helpers;
using uSupport.Dtos.Tables;
using uSupport.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using uSupport.Migrations.Schemas;
using Microsoft.Extensions.Logging;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace uSupport.Controllers
{
	public class uSupportTicketTypeAuthorizedApiController : UmbracoAuthorizedApiController
	{
		private readonly ILogger<IuSupportTicketTypeService> _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDataTypeService _dataTypeService;
		private readonly IuSupportTicketTypeService _uSupportTicketTypeService;
		
		public uSupportTicketTypeAuthorizedApiController(
			ILogger<IuSupportTicketTypeService> logger,
			IEventAggregator eventAggregator,
			IDataTypeService dataTypeService,
			IuSupportTicketTypeService uSupportTicketTypeService
			)
		{
            _eventAggregator = eventAggregator;
            _logger = logger;
			_dataTypeService = dataTypeService;
			_uSupportTicketTypeService = uSupportTicketTypeService;
		}

		[HttpGet]
		public IEnumerable<string> GetAllDataTypes()
		{
			string[] excludedDataTypes = new string[]
			{
				"Umbraco.MultiNodeTreePicker,",
				"Umbraco.MemberGroupPicker",
				"Umbraco.NestedContent",
				"Umbraco.MediaPicker3",
				"Umbraco.MemberPicker",
				"Umbraco.UserPicker",
				"Umbraco.BlockList",
				"Umbraco.ListView",
				"Umbraco.Label",
				"Umbraco.Grid",			
			};

			var dataTypes = _dataTypeService.GetAll()
												.Where(x => !excludedDataTypes.Contains(x.EditorAlias))
												.Select(x => x.Name).ToList();
			dataTypes.Sort();

			return dataTypes;
		}

		[HttpGet]
		public IDataType GetDataTypeFromId(int id) => _dataTypeService.GetDataType(id);

		[HttpGet]
		public IDataType GetDataTypeFromName(string name) => _dataTypeService.GetDataType(name);

		[HttpGet]
		public string GetDataTypeViewFromEditorAlias(string editorAlias) {
			var arr = editorAlias.Split('.').Skip(1);

			return arr.FirstOrDefault().ToLower() + string.Join("", arr.Skip(1));
		}

		[HttpGet]
		public IEnumerable<uSupportTicketType> GetAllTicketTypes() => _uSupportTicketTypeService.GetAll();

		[HttpGet]
		public uSupportTicketType GetTicketType(Guid id) => _uSupportTicketTypeService.Get(id);

		[HttpPost]
		public IEnumerable<uSupportTicketType> GetTicketTypes(List<Guid> ids)
        {
			if (ids == null) return null;

			return _uSupportTicketTypeService.GetByIds(ids);
		} 
			
		[HttpGet]
		public Guid GetTypeIdFromName(string name) => _uSupportTicketTypeService.GetTypeIdFromName(name);


		[HttpPost]
		public ActionResult<uSupportTicketType> CreateTicketType(uSupportTicketTypeSchema ticketType)
		{
            try
            {
				ticketType.Order = _uSupportTicketTypeService.GetTypesCount() + 1;
				var type = _uSupportTicketTypeService.Create(ticketType);
				_eventAggregator.Publish(new CreateTicketTypeNotification(type));

                return type;
			}
            catch (Exception ex)
            {
				_logger.LogError(ex, "Failed to create ticket type.");
				return ValidationProblem("Failed to create ticket type.");
			}
		}

		[HttpPost]
		public ActionResult<uSupportTicketType> UpdateTicketType(uSupportTicketType ticketType)
		{
            try
            {
				var type = _uSupportTicketTypeService.Update(ticketType.ConvertDtoToSchema());
				_eventAggregator.Publish(new UpdateTicketTypeNotification(type));

                return type;
			}
            catch (Exception ex)
            {
				_logger.LogError(ex, "Failed to update ticket type '{TicketTypeName}'",ticketType.Name);
				return ValidationProblem("Failed to update ticket type.");
			}			
		}

		[HttpGet]
		public void DeleteTicket(Guid id)
		{
            var ticketType = _uSupportTicketTypeService.Get(id);
            _eventAggregator.Publish(new DeleteTicketTypeNotification(ticketType));
            _uSupportTicketTypeService.Delete(id);
		}
	}
}
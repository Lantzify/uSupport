using uSupport.Dtos;
using uSupport.Helpers;
using Umbraco.Extensions;
using uSupport.Constants;
using uSupport.Dtos.Tables;
using Microsoft.AspNetCore.Mvc;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace uSupport.Controllers
{
	public class uSupportActionAuthorizedApiController : UmbracoAuthorizedApiController
	{
		private readonly IuSupportTicketService _uSupportTicketService;
		private readonly IuSupportTicketTypeService _uSupportTicketTypeService;
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;

		public uSupportActionAuthorizedApiController(IuSupportTicketService uSupportTicketService, 
			IuSupportTicketTypeService uSupportTicketTypeService,
			IuSupportTicketStatusService uSupportTicketStatusService)
		{
			_uSupportTicketService = uSupportTicketService;
			_uSupportTicketTypeService = uSupportTicketTypeService;
			_uSupportTicketStatusService = uSupportTicketStatusService;
		}

		[HttpPost]
		public void Delete (DeleteActionDto dto)
        {
            switch (dto.Type)
            {
				case uSupportConstants.TicketsTreeAlias:
					_uSupportTicketService.Delete(dto.Id);
                    _uSupportTicketService.ClearTicketCache();
                    break;
				case uSupportConstants.TicketTypesTreeAlias:
					_uSupportTicketTypeService.Delete(dto.Id);
					break;
				case uSupportConstants.TicketStatusesTreeAlias:
					_uSupportTicketStatusService.Delete(dto.Id);
					break;
			};
		}

		[HttpPost]
		public IEnumerable<PreviewDeleteItemDto> PreviewDelete(DeleteActionDto dto)
        {
            switch (dto.Type)
            {
				case uSupportConstants.TicketTypesTreeAlias:
					return _uSupportTicketService.GetAll()?.Where(x => x.TypeId == dto.Id).ConvertToDeletePreviewDto();
				case uSupportConstants.TicketStatusesTreeAlias:
					return _uSupportTicketService.GetAll()?.Where(x => x.StatusId == dto.Id).ConvertToDeletePreviewDto();
				default:
					return new List<PreviewDeleteItemDto>();

			};
		}

		[HttpGet]
		public IEnumerable<uSupportTypeBase> GetChildren(string type)
        {
			var list = new List<uSupportTypeBase>();

			switch (type)
			{
				case uSupportConstants.TicketTypesTreeAlias:
					foreach (uSupportTicketType status in _uSupportTicketTypeService.GetAll())
					{
						list.Add(new uSupportTypeBase()
						{
							Id = status.Id,
							Name = status.Name,
							Order = status.Order
						});
					}
					break;
				case uSupportConstants.TicketStatusesTreeAlias:
					foreach (uSupportTicketStatus status in _uSupportTicketStatusService.GetAll())
					{
						list.Add(new uSupportTypeBase()
						{
							Id = status.Id,
							Name = status.Name,
							Order = status.Order
						});
					}
					break;
			}

			return list;
		}

		[HttpPost]
		public void Sort(SortActionDto dto)
        {
			switch (dto.Type)
			{
				case uSupportConstants.TicketTypesTreeAlias:

					foreach (var item in dto.List)
					{
						var type = _uSupportTicketTypeService.Get(item.Id);
						type.Order = dto.List.FindIndex(x => x.Id == item.Id) + 1;
						_uSupportTicketTypeService.Update(type.ConvertDtoToSchema());
					}

					break;
				case uSupportConstants.TicketStatusesTreeAlias:

					foreach (var item in dto.List)
					{
						var status = _uSupportTicketStatusService.Get(item.Id);
						status.Order = dto.List.FindIndex(x => x.Id == item.Id) + 1;
						_uSupportTicketStatusService.Update(status.ConvertDtoToSchema());
					}

					break;
			}
		}
	}
}
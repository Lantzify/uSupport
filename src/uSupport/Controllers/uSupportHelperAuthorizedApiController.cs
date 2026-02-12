using uSupport.Dtos;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.ContentApps;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace uSupport.Controllers
{
	public class uSupportHelperAuthorizedApiController : UmbracoAuthorizedApiController
	{
		private readonly IUserService _userService;
        private readonly IUmbracoMapper _umbracoMapper;
		private readonly IEntityService _entityService;
        private readonly ContentAppFactoryCollection _contentAppDefinitions;

        public uSupportHelperAuthorizedApiController(IUserService userService,
            ContentAppFactoryCollection contentAppDefinitions,
        IUmbracoMapper umbracoMapper,
		IEntityService entityService)
		{
			_userService = userService;
			_umbracoMapper = umbracoMapper;
			_entityService = entityService;
			_contentAppDefinitions = contentAppDefinitions;
		}

        [HttpPost]
        public IEnumerable<ContentApp> GetAddons(object type) => _contentAppDefinitions.GetContentAppsFor(type);

        [HttpGet]
		public string GenerateExternalTicketId() => $"{GenerateRandomLetters(3)}-{GenerateRandomLetters(4)}-{GenerateRandomNumbers(3)}";

		[HttpGet]
		public UserDisplay GetUserById(int id)
		{
			var user = _userService.GetUserById(id);
			var result = _umbracoMapper.Map<IUser, UserDisplay>(user);
			return result;
		}

		[HttpPost]
		public UserDisplay SaveUser(uSupportUserSaveDto uSupportUserSaveDto)
		{
			var user = _userService.GetUserById(uSupportUserSaveDto.Id);

			user.Name = uSupportUserSaveDto.Name;
			user.Email = uSupportUserSaveDto.Email;

			_userService.Save(user);

			return GetUserById(user.Id);
		}

		[HttpPost]
		public IUser ClearAvatar(UserDisplay displayUser)
		{
			var user = _userService.GetUserById(int.Parse(displayUser.Id.ToString()));
			user.Avatar = "";

			_userService.Save(user);

			return user;
		}

		[HttpPost]
		public object ReadOnlyValue(object value)
		{
			if (value != null)
			{
				switch (Type.GetTypeCode(value.GetType()))
				{
					case TypeCode.Object:
						return JsonConvert.DeserializeObject<IEnumerable<uSupportReadonlyDto>>($"{value}");

					default:
						int id = int.Parse($"{value}");
						UmbracoObjectTypes objectType = _entityService.GetObjectType(id);
						return new List<object> ()
						{
							_entityService.Get(id, objectType)
						};
				}
			}

			return value;
		}

		private string GenerateRandomNumbers(int length)
		{
			var sequence = Enumerable.Range(1, 9).OrderBy(n => n * n * (new Random()).Next());

			return string.Join("", sequence.Distinct().Take(length));
		}

		private string GenerateRandomLetters(int length)
		{
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}
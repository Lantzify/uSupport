using uSupport.Dtos;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Media;
using Umbraco.Cms.Core.Services;

namespace uSupport.Helpers
{
	public class uSupportUserHelper
	{
		private readonly AppCaches _appCaches;
		private readonly IUserService _userService;
		private readonly MediaFileManager _mediaFileManager;
		private readonly IImageUrlGenerator _imageUrlGenerator;

		public uSupportUserHelper(AppCaches appCaches,
			IUserService userService,
			MediaFileManager mediaFileManager,
			IImageUrlGenerator imageUrlGenerator)
		{
			_appCaches = appCaches;
			_userService = userService;
			_mediaFileManager = mediaFileManager;
			_imageUrlGenerator = imageUrlGenerator;
		}

		public uSupportUser? GetUser(int id)
		{
			var user = _userService.GetUserById(id);
			if (user == null)
				return null;

			return new uSupportUser
			{
				Id = user.Id,
				Key = user.Key,
				Name = user.Name,
				Email = user.Email,
				AvatarUrls = user.GetUserAvatarUrls(_appCaches.RuntimeCache, _mediaFileManager, _imageUrlGenerator)
			};
		}
	}
}

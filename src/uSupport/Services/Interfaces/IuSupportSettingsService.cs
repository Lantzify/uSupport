using uSupport.Dtos.Tables;
using uSupport.Migrations.Schemas;

namespace uSupport.Services.Interfaces
{
	public interface IuSupportSettingsService
	{
		void SendEmail(string toAddress, string subject, string templateViewPath, object model);
		uSupportSettings GetSettings();
		uSupportSettings Get(Guid id);
		uSupportSettings Create(uSupportSettingsSchema settings);
		uSupportSettings Update(uSupportSettingsSchema settings);
		void Delete(Guid id);
	}
}
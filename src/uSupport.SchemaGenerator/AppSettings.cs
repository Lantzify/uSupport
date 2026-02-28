using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uSupport.Dtos.Settings;
using System.Collections.Generic;

namespace uSupport.SchemaGenerator
{
	internal class AppSettings
	{
		public uSupportSettings uSupport { get; set; } = new ();

	}
}
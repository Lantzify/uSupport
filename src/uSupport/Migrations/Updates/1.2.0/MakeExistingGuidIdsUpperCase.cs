﻿#if NETCOREAPP
using Umbraco.Cms.Infrastructure.Migrations;
#else
using Umbraco.Core.Migrations;
#endif
using uSupport.Migrations.Schemas;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Updates._1._2._0
{
	public class MakeExistingGuidIdsUpperCase : uSupportMigrationBase
	{
		public MakeExistingGuidIdsUpperCase(IMigrationContext context) : base(context)
		{ }

		protected override void DoMigrate()
		{
			Execute.Sql($"UPDATE {TicketTypeTableAlias} SET Id = UPPER(Id)").Do();
			Execute.Sql($"UPDATE {TicketStatusTableAlias} SET Id = UPPER(Id)").Do();
		}
	}
}

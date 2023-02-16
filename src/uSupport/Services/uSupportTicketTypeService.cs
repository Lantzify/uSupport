﻿#if NET6_0_OR_GREATER
using Umbraco.Cms.Infrastructure.Scoping;
#elif NET5_0
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Core.Scoping;
#endif
using NPoco;
using System;
using System.Linq;
using uSupport.Dtos.Tables;
using System.Collections.Generic;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketTypeService : uSupportServiceBase<uSupportTicketType, uSupportTicketTypeSchema>, IuSupportTicketTypeService
	{
		private static IScopeProvider _scopeProvider;

		public uSupportTicketTypeService(IScopeProvider scopeProvider) : base(TicketTypeTableAlias, scopeProvider)
		{
			_scopeProvider = scopeProvider;
		}

		public IEnumerable<uSupportTicketType> GetAll()
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var sql = new Sql()
					.Select("*")
					.From(TicketTypeTableAlias)
					.OrderBy("[Order]");

				return scope.Database.Fetch<uSupportTicketType>(sql);
			}
		}

		public Guid GetTypeIdFromName(string name)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var ticketStatus = db.Query<uSupportTicketStatus>($"SELECT Id FROM {TicketTypeTableAlias} WHERE [Name] = @name", new { name }).FirstOrDefault();

				return ticketStatus.Id;
			}
		}

		public int GetTypesCount()
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				return db.Query<uSupportTicketType>($"SELECT [Order] FROM {TicketTypeTableAlias}").ToList().Count;
			}
		}
	}
}
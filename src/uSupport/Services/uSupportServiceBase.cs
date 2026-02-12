using NPoco;
using uSupport.Extensions;
using Umbraco.Cms.Infrastructure.Scoping;

namespace uSupport.Services.Interfaces
{
	public abstract class uSupportServiceBase<T, Schema>
	{
		private static IScopeProvider _scopeProvider;

		private readonly string _tableAlias;

		public uSupportServiceBase(string tableAlias, IScopeProvider scopeProvider)
		{
			_tableAlias = tableAlias;
			_scopeProvider = scopeProvider;
		}
		
		public virtual T Create(Schema dto)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				scope.Database.Insert(_tableAlias, "Id", false, dto);
				scope.Complete();
			}

			var dtoType = dto.GetType();
			var dtoIdProperty = dtoType.GetProperty("Id");
			var get = Get(Guid.Parse(dtoIdProperty.GetValue(dto).ToString()));

			return get;
		}

		public virtual T Get(Guid id)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var sql = new Sql()
					.Select("*")
					.From(_tableAlias)
					.Where($"Id = UPPER('{id}')");

				return scope.Database.Fetch<T>(sql).FirstOrDefault();
			}
		}

		public virtual IEnumerable<T> GetByIds(List<Guid> ids)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var sql = new Sql()
					.Select("*")
					.From(_tableAlias)
					.Where($"Id IN({ids.ConvertGuidToSqlString()})");

				return scope.Database.Fetch<T>(sql);
			}
		}

		public virtual T Update(Schema dto)
		{
			var dtoType = dto.GetType();
			var dtoIdProperty = dtoType.GetProperty("Id");
			var id = dtoIdProperty.GetValue(dto).ToString();

			using (var scope = _scopeProvider.CreateScope())
			{
				scope.Database.UpdateWhere(dto, $"Id = UPPER('{id}')");
				scope.Complete();
			}

			var updatedNotification = Get(Guid.Parse(id));

			return updatedNotification;
		}

		public void Delete(Guid id)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var result = scope.Database.Delete<T>($"WHERE Id = UPPER('{id}')");
				scope.Complete();
			}
		}
	}
}
using NPoco;
using uSupport.Extensions;
using Umbraco.Cms.Infrastructure.Scoping;

namespace uSupport.Services.Interfaces
{
	public abstract class uSupportServiceBase<T, Schema>
	{
		private readonly IScopeProvider _scopeProvider;
		private readonly IScopeAccessor _scopeAccessor;

		private readonly string _tableAlias;

		public uSupportServiceBase(string tableAlias,
			IScopeProvider scopeProvider,
			IScopeAccessor scopeAccessor)
		{
			_tableAlias = tableAlias;
			_scopeProvider = scopeProvider;
			_scopeAccessor = scopeAccessor;
		}

		public virtual T Create(Schema dto)
		{
			var context = GetScope();

			try
			{
				context.Scope.Database.Insert(_tableAlias, "Id", false, dto);

				if (context.Created)
					context.Scope.Complete();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}

			var dtoType = dto.GetType();
			var dtoIdProperty = dtoType.GetProperty("Id");

			return Get(Guid.Parse(dtoIdProperty.GetValue(dto).ToString()));
		}

		public virtual T Get(Guid id)
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
							.Select("*")
							.From(_tableAlias)
							.Where($"Id = UPPER('{id}')");

				return context.Scope.Database.Fetch<T>(sql).FirstOrDefault();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public virtual IEnumerable<T> GetByIds(List<Guid> ids)
		{
			var context = GetScope();
			try
			{
				var sql = new Sql()
								.Select("*")
								.From(_tableAlias)
								.Where($"Id IN({ids.ConvertGuidToSqlString()})");

				return context.Scope.Database.Fetch<T>(sql);
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		public virtual T Update(Schema dto)
		{
			var context = GetScope();

			var dtoType = dto.GetType();
			var dtoIdProperty = dtoType.GetProperty("Id");
			var id = Guid.Parse(dtoIdProperty.GetValue(dto).ToString());

			try
			{
				context.Scope.Database.UpdateWhere(dto, $"Id = UPPER('{id}')");
				if (context.Created)
					context.Scope.Complete();

			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}

			return Get(id);
		}

		public virtual void Delete(Guid id)
		{
			var context = GetScope();
			try
			{
				context.Scope.Database.Delete<T>($"WHERE Id = UPPER('{id}')");

				if (context.Created)
					context.Scope.Complete();
			}
			finally
			{
				if (context.Created)
					context.Scope.Dispose();
			}
		}

		protected ScopeContext GetScope()
		{
			var ambient = _scopeAccessor.AmbientScope;

			if (ambient != null)
				return new ScopeContext(ambient, false);

			return new ScopeContext(_scopeProvider.CreateScope(), true);
		}

		protected class ScopeContext
		{
			public IScope Scope { get; }
			public bool Created { get; }

			public ScopeContext(IScope scope, bool created)
			{
				Scope = scope;
				Created = created;
			}
		}
	}
}
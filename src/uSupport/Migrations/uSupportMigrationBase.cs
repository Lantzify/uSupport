using Umbraco.Cms.Infrastructure.Migrations;

namespace uSupport.Migrations
{
    public abstract class uSupportMigrationBase : AsyncMigrationBase
    {
        public uSupportMigrationBase(IMigrationContext context) : base(context) { }

        protected abstract void DoMigrate();

        protected override Task MigrateAsync()
        {
            DoMigrate();
            return Task.CompletedTask;
        }
    }
}

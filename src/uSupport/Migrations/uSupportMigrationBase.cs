using Umbraco.Cms.Infrastructure.Migrations;

namespace uSupport.Migrations
{
    public abstract class uSupportMigrationBase : MigrationBase
    {
        public uSupportMigrationBase(IMigrationContext context) : base(context) { }

        protected abstract void DoMigrate();

        protected override void Migrate() => DoMigrate();
    }
}
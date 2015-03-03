using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;

namespace TwolipsDating.Migrations
{
    public static class MigrationExtensions
    {
        public static IdentityChangeOperationWrapper ChangeIdentity(
            this DbMigration migration,
            IdentityChange change,
            string principalTable,
            string principalColumn)
        {
            var operation = new ChangeIdentityOperation
            {
                Change = change,
                PrincipalTable = principalTable,
                PrincipalColumn = principalColumn,
                DependentColumns = new List<DependentColumn>()
            };

            ((IDbMigration)migration).AddOperation(operation);

            return new IdentityChangeOperationWrapper(operation);
        }

        public class IdentityChangeOperationWrapper
        {
            private ChangeIdentityOperation _operation;

            public IdentityChangeOperationWrapper(ChangeIdentityOperation operation)
            {
                _operation = operation;
            }

            public IdentityChangeOperationWrapper WithDependentColumn(
                string table,
                string foreignKeyColumn)
            {
                _operation.DependentColumns.Add(new DependentColumn
                {
                    DependentTable = table,
                    ForeignKeyColumn = foreignKeyColumn
                });

                return this;
            }
        }
    }
}
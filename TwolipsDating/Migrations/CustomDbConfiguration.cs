using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TwolipsDating.Migrations
{
    /// <summary>
    /// Custom DbConfiguration sets a custom SQL generator for the code-first migration framework. This class seems to be found via
    /// reflection by Entity Framework (as long as this class is in the same assembly as the DbContext).
    /// </summary>
    public class CustomDbConfiguration : DbConfiguration
    {
        public CustomDbConfiguration()
        {
            SetMigrationSqlGenerator("System.Data.SqlClient", () => new MySqlServerMigrationSqlGenerator());
        }
    }
}
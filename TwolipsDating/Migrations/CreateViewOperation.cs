using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;

namespace TwolipsDating.Migrations
{
    /// <summary>
    /// Custom code-first migration operation to create a view based on a name and body.
    /// </summary>
    public class CreateViewOperation : MigrationOperation
    {
        public string ViewName { get; private set; }

        public string ViewString { get; private set; }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }

        public CreateViewOperation(string viewName, string viewQueryString)
            : base(null)
        {
            ViewName = viewName;
            ViewString = viewQueryString;
        }
    }
}
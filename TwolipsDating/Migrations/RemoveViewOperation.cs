using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;

namespace TwolipsDating.Migrations
{
    public class RemoveViewOperation : MigrationOperation
    {
        public string ViewName { get; private set; }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }

        public RemoveViewOperation(string viewName)
            : base(null)
        {
            ViewName = viewName;
        }
    }
}
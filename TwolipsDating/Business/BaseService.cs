using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class BaseService
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}
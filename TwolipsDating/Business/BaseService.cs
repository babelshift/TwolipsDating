using System;
using System.Collections.Generic;
using TwolipsDating.Models;
using System.Threading.Tasks;
using System.Diagnostics;
using Dapper;
using System.Data.SqlClient;

namespace TwolipsDating.Business
{
    public class BaseService
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters)
        {
            Debug.Assert(!String.IsNullOrEmpty(sql));
            Debug.Assert(parameters != null);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var results = await db.Database.Connection.QueryAsync<T>(sql, parameters);
                connection.Close();
                return results;
            }
        }



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
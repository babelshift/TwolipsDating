using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class BaseService
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        public BaseService()
        {
#if DEBUG
            db.Database.Log = s => { Debug.WriteLine(s); };
#endif
        }

        public BaseService(ApplicationDbContext db) : this()
        {
            this.db = db;
        }

        /// <summary>
        /// Uses Dapper to perform an asynchronous query to the default connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Disposes the db context
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the db context
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}
using Dapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.Utilities;

namespace TwolipsDating.Business
{
    public class BaseService : IDisposable
    {
        private IValidationDictionary validationDictionary;
        private IIdentityMessageService emailService;
        protected ApplicationDbContext db = new ApplicationDbContext();
        protected LogHelper Log { get; private set; }

        protected IIdentityMessageService EmailService { get { return emailService; } }

        protected IValidationDictionary ValidationDictionary { get { return validationDictionary; } }

        public BaseService()
        {
            Log = new LogHelper(GetType().FullName);
#if DEBUG
            db.Database.Log = s => { Debug.WriteLine(s); };
#endif
        }

        public BaseService(IValidationDictionary validationDictionary)
            : this()
        {
            this.validationDictionary = validationDictionary;
        }

        public BaseService(IIdentityMessageService emailService)
            : this()
        {
            this.emailService = emailService;
        }

        public BaseService(IIdentityMessageService emailService, IValidationDictionary validationDictionary)
            : this()
        {
            this.emailService = emailService;
            this.validationDictionary = validationDictionary;
        }

        public BaseService(ApplicationDbContext db, IIdentityMessageService emailService)
            : this(emailService)
        {
            this.db = db;
        }

        public BaseService(ApplicationDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// This will commit any pending transaction.
        /// </summary>
        /// <param name="fromUserId"></param>
        /// <param name="milestoneTypeId"></param>
        /// <returns></returns>
        protected async Task AwardAchievedMilestonesForUserAsync(string fromUserId, int milestoneTypeId)
        {
            // handle and save any milestones that the user may have met
            MilestoneService milestoneService = new MilestoneService(db, emailService);
            await milestoneService.AwardAchievedMilestonesAsync(fromUserId, milestoneTypeId);
            await db.SaveChangesAsync();
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
                return results;
            }
        }

        protected async Task<int> ExecuteAsync(string sql, object parameters)
        {
            Debug.Assert(!String.IsNullOrEmpty(sql));
            Debug.Assert(parameters != null);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int count = await db.Database.Connection.ExecuteAsync(sql, parameters);
                    return count;
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex.Message, ex, parameters);
            }

            return 0;
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
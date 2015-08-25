using Dapper;
using Microsoft.AspNet.Identity;
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
        private string profileIndexUrlRoot;
        private IIdentityMessageService emailService;
        protected ApplicationDbContext db = new ApplicationDbContext();

        protected IIdentityMessageService EmailService { get { return emailService; } }

        protected string ProfileIndexUrlRoot { get { return profileIndexUrlRoot; } }

        public BaseService()
        {
#if DEBUG
            db.Database.Log = s => { Debug.WriteLine(s); };
#endif
        }

        public BaseService(IIdentityMessageService emailService, string profileIndexUrlRoot)
            : this()
        {
            this.emailService = emailService;
            this.profileIndexUrlRoot = profileIndexUrlRoot;
        }

        public BaseService(ApplicationDbContext db, IIdentityMessageService emailService, string profileIndexUrlRoot)
            : this()
        {
            this.db = db;
            this.emailService = emailService;
            this.profileIndexUrlRoot = profileIndexUrlRoot;
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
            MilestoneService milestoneService = new MilestoneService(db, emailService, profileIndexUrlRoot);
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
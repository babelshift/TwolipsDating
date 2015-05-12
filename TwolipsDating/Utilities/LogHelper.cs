using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace TwolipsDating.Utilities
{
    public class LogHelper
    {
        private Logger logger;

        public LogHelper(string typeName)
        {
            logger = LogManager.GetLogger(typeName);
        }
        
        public void Error(string message, string stackTrace)
        {
            Debug.Assert(!String.IsNullOrEmpty(message));
            Debug.Assert(!String.IsNullOrEmpty(stackTrace));

            StringBuilder logMessage = new StringBuilder();

            logMessage.AppendFormat("Message: {0}", message);
            logMessage.AppendFormat(", StackTrace: {0}", stackTrace);

            logger.Error(logMessage.ToString());
        }

        public void Info(string message)
        {
            Debug.Assert(!String.IsNullOrEmpty(message));

            logger.Info(message);
        }

        public void Warn(string actionName, string message, object parameters = null)
        {
            Debug.Assert(!String.IsNullOrEmpty(actionName));
            Debug.Assert(!String.IsNullOrEmpty(message));

            StringBuilder logMessage = new StringBuilder();

            logMessage.AppendFormat("Action: {0}", actionName);
            logMessage.AppendFormat(", Message: {0}", message);

            if (parameters != null)
            {
                logMessage.Append(", Params: [");
                Type anonType = parameters.GetType();
                foreach (var property in anonType.GetProperties())
                {
                    logMessage.AppendFormat("{0}: {1}", property.Name, property.GetValue(parameters, null));
                }
                logMessage.Append("]");
            }

            logger.Warn(logMessage.ToString());
        }

        public void Error(string actionName, Exception e, object parameters = null)
        {
            Debug.Assert(!String.IsNullOrEmpty(actionName));
            Debug.Assert(e != null);

            StringBuilder logMessage = new StringBuilder();

            logMessage.AppendFormat("Action: {0}", actionName);
            logMessage.AppendFormat(", Message: {0}", e.Message);

            if(e.InnerException != null)
            {
                logMessage.AppendFormat(", InnerMessage: {0}", e.InnerException.Message);

                if(e.InnerException.InnerException != null)
                {
                    logMessage.AppendFormat(", InnerInnerMessage: {0}", e.InnerException.Message);
                }
            }

            if (parameters != null)
            {
                logMessage.Append(", Params: [");
                Type anonType = parameters.GetType();
                foreach (var property in anonType.GetProperties())
                {
                    logMessage.AppendFormat("{0}: {1}, ", property.Name, property.GetValue(parameters, null));
                }
                logMessage.Append("]");
            }

                logMessage.AppendFormat(", StackTrace: {0}", e.StackTrace);

            logger.Error(logMessage.ToString());
        }
    }
}
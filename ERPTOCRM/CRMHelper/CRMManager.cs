using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Net;
using System.Text;

namespace CRMHelper
{
    public class CRMManager
    {
        public static IOrganizationService GetCRMService(string crmURL)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var service = new CrmServiceClient(crmURL);
                return service;
            }
            catch (Exception ex)
            {
                StringBuilder strErrorDetails = new StringBuilder();
                strErrorDetails.AppendFormat(" Organization URL: {0}", crmURL);

                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                    strErrorDetails.AppendLine(string.Format("InnerException: {0}", ex.InnerException.Message));
                if (!string.IsNullOrEmpty(ex.Message))
                    strErrorDetails.AppendLine(string.Format("Message: {0}", ex.Message));
                if (!string.IsNullOrEmpty(ex.StackTrace))
                    strErrorDetails.AppendLine(string.Format("StackTrace: {0}", ex.StackTrace));

                throw new Exception($"GetCRMService: Error occurred in GetCRMService. {strErrorDetails.ToString()}", ex);
            }
        }
    }
}

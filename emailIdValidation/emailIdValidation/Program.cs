using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace emailIdValidation
{
    class Program
    {
        static void Main(string[] args)
        {
            IOrganizationService service = GetCRMService("Url=https://cutm0.crm8.dynamics.com/;UserName=140301csr008@cutm.ac.in;Password=L@l1th@2020;AuthType=Office365;");

            Entity account = service.Retrieve("account", new Guid("135FC249-86B5-E811-A961-000D3AF04FC2"), new ColumnSet(true));

            Entity createAccount = new Entity("account");
            createAccount["name"] = "Email Validation";
            createAccount["emailaddress1"] = "emailvalidation.com";
            service.Create(createAccount);
            //string email = string.Empty;
            //Entity update = new Entity("account");
            //update.Id = account.Id;
            //if (account.Attributes.Contains("emailaddress1"))
            //    email = account.Attributes["emailaddress1"].ToString();
            //if (ValidateEmail(email))
            //{
            //    update["name"] = "Checked Valid Mail";
            //    service.Update(update);
            //}
            //else
            //{
            //    throw new InvalidPluginExecutionException("Invalid Email Address");
            //}
        }
        //private static bool ValidateEmail(string email)
        //{
        //    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        //    Match match = regex.Match(email);
        //    if (match.Success)
        //        return true;
        //    return false;
        //}
        private static IOrganizationService GetCRMService(string crmURL)
        {
            try
            {
                // For Dynamics 365 Customer Engagement V9.X, set Security Protocol as TLS12
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var service = new CrmServiceClient(crmURL);
                return service;
            }
            catch (Exception e)
            {
                StringBuilder strErrorDetails = new StringBuilder();
                strErrorDetails.AppendFormat(" Organization URL: {0}", crmURL);

                if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
                    strErrorDetails.AppendLine(string.Format("InnerException: {0}", e.InnerException.Message));
                if (!string.IsNullOrEmpty(e.Message))
                    strErrorDetails.AppendLine(string.Format("Message: {0}", e.Message));
                if (!string.IsNullOrEmpty(e.StackTrace))
                    strErrorDetails.AppendLine(string.Format("StackTrace: {0}", e.StackTrace));

                throw new Exception($"Error occurred in GetCRMService. {strErrorDetails.ToString()}", e);
            }
        }
    }
}

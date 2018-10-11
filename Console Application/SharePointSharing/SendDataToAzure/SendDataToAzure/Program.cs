using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SendDataToAzure
{
    class Program
    {
        static void Main(string[] args)
        {
            IOrganizationService service = GetCRMService("Url=https://devchecklist.crm4.dynamics.com/;UserName=hardik@avantit.no;Password=WdScbdQK12$;AuthType=Office365;");

            Entity connection = service.Retrieve("connection", new Guid("4D0ED81A-5C95-E811-A94E-000D3A39C2C9"), new ColumnSet(true));
            string messageName = "Create";

            EntityReference record2RoleId = null;
            if (connection.Attributes.Contains("record2roleid"))
                record2RoleId = (EntityReference)connection["record2roleid"];


            EntityReference record1Id = null;
            if (connection.Attributes.Contains("record1id"))
                record1Id = (EntityReference)connection["record1id"];

            EntityReference record2Id = null;
            if (connection.Attributes.Contains("record2id"))
                record2Id = (EntityReference)connection["record2id"];

            SharePointSharing sharePointSharing = new SharePointSharing();
            if (record2Id != null && record2RoleId != null && (record2RoleId.Id == new Guid("D6F5C43F-D594-E811-810E-3863BB35AF60") || record2RoleId.Id == new Guid("4A06D021-D594-E811-810E-3863BB35AF60")))
            {
                sharePointSharing.Permission = record2RoleId.Name;

                Entity relatedEntity = service.Retrieve(record2Id.LogicalName, record2Id.Id, new ColumnSet("emailaddress1"));
                if (relatedEntity.Attributes.Contains("emailaddress1"))
                    sharePointSharing.ExternalUserEmail = Convert.ToString(relatedEntity.Attributes["emailaddress1"]);

                if (connection.Attributes.Contains("description"))
                    sharePointSharing.FolderName = Convert.ToString(connection.Attributes["description"]);

                if (messageName == "Create")
                    sharePointSharing.Operation = "Share";

            }
        }
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

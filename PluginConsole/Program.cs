using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PluginConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IOrganizationService service = GetCRMService("Url=https://friggdevelopment.crm4.dynamics.com/;UserName=niklas.odegaard@gronnkontakt.no;Password=SkyMannen2018;AuthType=Office365;");

            Entity location = service.Retrieve("gk_location", new Guid("6158468F-E2C6-E811-A841-000D3A2A78DB"), new ColumnSet(true));

            if (location.Attributes.Contains("statuscode"))
            {
                int selectedValue = ((OptionSetValue)location.Attributes["statuscode"]).Value;
                GetOptionsSetTextForValue(service, location.LogicalName, "statuscode", selectedValue);
                string str = GetOptionSetText(location, "statuscode");
            }

            //if (obj.Attributes.Contains("parentcustomerid"))
            //{
            //    EntityReference refAccount = (EntityReference)obj.Attributes["parentcustomerid"];
            //    Entity objAccount = service.Retrieve(refAccount.LogicalName, refAccount.Id, new ColumnSet(true));

            //    Entity contact = new Entity("contact");
            //    contact.Id = new Guid("465B158C541CE51180D33863BB347BA8");

            //    if (objAccount.Attributes.Contains("address1_line1"))
            //        contact["address1_line1"] = objAccount["address1_line1"];

            //    if (objAccount.Attributes.Contains("address1_line2"))
            //        contact["address1_line2"] = objAccount["address1_line2"];

            //    if (objAccount.Attributes.Contains("address1_line3"))
            //        contact["address1_line3"] = objAccount["address1_line3"];

            //    if (objAccount.Attributes.Contains("address1_stateorprovince"))
            //        contact["address1_stateorprovince"] = objAccount["address1_stateorprovince"];

            //    if (objAccount.Attributes.Contains("address1_stateorprovince"))
            //        contact["address1_postalcode"] = objAccount["address1_stateorprovince"];

            //    if (objAccount.Attributes.Contains("address1_country"))
            //        contact["address1_country"] = objAccount["address1_country"];

            //    service.Update(contact);
            //}
            //return;
        }


        public static string GetOptionSetText(Entity entity, string attributeName)
        {
            string value = string.Empty;
            if (entity.FormattedValues.ContainsKey(attributeName))
            {
                value = entity.FormattedValues[attributeName].ToString();
            }
            return value;
        }
        public static string GetOptionsSetTextForValue(IOrganizationService service, string entityName, string attributeName, int selectedValue)
        {
            RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };

            // Execute the request.
            RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);
            // Access the retrieved attribute.
            StatusAttributeMetadata retrievedPicklistAttributeMetadata = (StatusAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;// Get the current options list for the retrieved attribute.
            OptionMetadata[] optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
            string selectedOptionLabel = null;
            foreach (OptionMetadata oMD in optionList)
            {
                if (oMD.Value == selectedValue)
                {
                    selectedOptionLabel = oMD.Label.LocalizedLabels[0].Label.ToString();
                    break;
                }
            }
            return selectedOptionLabel;
        }
        public static IOrganizationService GetCRMService(string crmURL)
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

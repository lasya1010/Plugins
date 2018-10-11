using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointSharing
{
    public class SendDataToAzure : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory servicefactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = servicefactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity templateConfiguration = null;
            string operation = string.Empty;

            if (context.MessageName == "Create")
            {
                templateConfiguration = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));
                operation = "Share";
            }
            else if(context.MessageName == "Delete")
            {                
                if (context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"] is Entity)
                {
                    templateConfiguration = (Entity)context.PreEntityImages["PreImage"];
                    operation = "Unshare";
                }
            }

            if (templateConfiguration != null)
            {
                EntityReference record2RoleId = null;
                if (templateConfiguration.Attributes.Contains("record2roleid"))
                    record2RoleId = (EntityReference)templateConfiguration["record2roleid"];

                EntityReference record1Id = null;
                if (templateConfiguration.Attributes.Contains("record1id"))
                    record1Id = (EntityReference)templateConfiguration["record1id"];

                EntityReference record2Id = null;
                if (templateConfiguration.Attributes.Contains("record2id"))
                    record2Id = (EntityReference)templateConfiguration["record2id"];

                SharePointSharing sharePointSharing = new SharePointSharing();
                if (record2Id != null && record2RoleId != null && (record2RoleId.Id == new Guid("D6F5C43F-D594-E811-810E-3863BB35AF60") || record2RoleId.Id == new Guid("4A06D021-D594-E811-810E-3863BB35AF60")))
                {
                    sharePointSharing.Permission = record2RoleId.Name;

                    Entity relatedEntity = service.Retrieve(record2Id.LogicalName, record2Id.Id, new ColumnSet("emailaddress1"));

                    if (relatedEntity.Attributes.Contains("emailaddress1"))
                        sharePointSharing.ExternalUserEmail = Convert.ToString(relatedEntity.Attributes["emailaddress1"]);

                    if (templateConfiguration.Attributes.Contains("description"))
                        sharePointSharing.FolderName = Convert.ToString(templateConfiguration.Attributes["description"]);

                    sharePointSharing.Operation = operation;
                    sharePointSharing.OrgUniqueName = context.OrganizationName;

                    //TODO: Get SharePoint URL and passed as JSON object to Azure Service BUS

                }
            }
        }
    }
}

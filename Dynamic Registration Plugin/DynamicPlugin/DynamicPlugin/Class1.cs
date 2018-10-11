using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicPlugin
{
    public class DynamicPluginclass : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory servicefactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = servicefactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity obj = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));


            if (obj.Attributes.Contains("productid"))
            {
                EntityReference refAccount = (EntityReference)obj.Attributes["productid"];
                Entity objAccount = service.Retrieve(refAccount.LogicalName, refAccount.Id, new ColumnSet(true));

                Entity Case = new Entity("incident");
                Case.Id = context.PrimaryEntityId;

                if (objAccount.Attributes.Contains("productnumber"))
                    Case["description"] = objAccount["productnumber"];

                service.Update(Case);
            }

            return;

        }
    }
}



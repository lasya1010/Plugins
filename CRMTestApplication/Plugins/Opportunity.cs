using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace SamplePlugins
{
    public class Opportunity : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = factory.CreateOrganizationService(context.UserId);
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                Entity obj = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));



                Entity con = new Entity("newOpportunity");
                con["name"] = "Sales";
                con["msdyn_ordertype"] = new OptionSetValue(690970002);
                con["customerneed"] = "Update the new Opportunities";
                con["parentcontactid"] = new EntityReference(context.PrimaryEntityName, context.PrimaryEntityId);
                service.Create(con);

            }
            catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace SamplePlugins
{
    public class PostCreateContact : IPlugin
    { 
        public void Execute( IServiceProvider serviceProvider)
        {
            
            IPluginExecutionContext context = (IPluginExecutionContext) serviceProvider.GetService(typeof(IPluginExecutionContext));
  
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity) {

                Entity entity = (Entity)context.InputParameters["Target"];
                try
                {
                    Entity followup = new Entity("task");
                    followup["subject"] = "Send e-mail to the new customer.";
                    followup["description"] = "Follow up with the customer. Check if there are any new issues that need resolution.";
    

                    followup["scheduledstart"] = DateTime.Now;
                    followup["scheduledend"] = DateTime.Now.AddDays(2);
                    followup["category"] = context.PrimaryEntityName;

                  
                    if (context.OutputParameters.Contains("id"))
                    {
                        Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                        string regardingobjectidType = "contact";
                        followup["regardingobjectid"] = new EntityReference(regardingobjectidType, regardingobjectid);
                    }
                    
                    IOrganizationServiceFactory serviceFactory =
                       (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service =
                       serviceFactory.CreateOrganizationService(context.UserId);

                    
                    service.Create(followup);
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }
    }
}

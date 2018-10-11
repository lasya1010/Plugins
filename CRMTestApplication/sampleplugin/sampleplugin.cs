using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleplugin
{
    public class sampleplugin : IPlugin
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


                //Entity task = new Entity("task");
                //task["subject"] = obj["fullname"];
                //task["description"] = "Created from Contact";
                //task["regardingobjectid"] = new EntityReference(context.PrimaryEntityName, context.PrimaryEntityId);
                //service.Create(task);

                Entity contact = new Entity("contact");
                contact["firstname"] = "Test";
                contact["lastname"] = "Account Contact";
                contact["mobilephone"] = "753159";
                contact["emailaddress1"] = "test@test.com";                
                contact["parentcustomerid"] = new EntityReference(obj.LogicalName, obj.Id);
                service.Create(contact);

            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException($"Error Occurred in sampleplugin. Message: {e.Message}, StackTrace: {e.StackTrace}");
            }
        }
    }
}

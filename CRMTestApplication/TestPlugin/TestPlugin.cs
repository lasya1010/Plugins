using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;

namespace TestPlugin
{
    public class TestPlugin : IPlugin //CodeActivity
    {
        // protected override void Execute(CodeActivityContext codeContext)
        public void Execute(IServiceProvider serviceProvider)
        {

            try
            {
               //  ITracingService traceService = codeContext.GetExtension<ITracingService>(); 
               //IWorkflowContext context = codeContext.GetExtension<IWorkflowContext>();
               // IOrganizationServiceFactory serviceFactory = codeContext.GetExtension<IOrganizationServiceFactory>();
               // IOrganizationService  service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);
               

                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = factory.CreateOrganizationService(context.UserId);
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                Entity obj = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));

                Entity con = new Entity("contact");
                con["firstname"] = "Test";
                con["lastname"] = "Account Contact";
                con["mobilephone"] = "753159";
                con["emailaddress1"] = "test@test.com";
                con["parentaccountid"] = new EntityReference(context.PrimaryEntityName, context.PrimaryEntityId);
                service.Create(con);

                //if (!obj.Attributes.Contains("telephone1"))
                //{
                //    throw new InvalidPluginExecutionException("Enter telephone1 field value");
                //} 


            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException($"Error Occurred in TestPlugin. Message: {e.Message}, StackTrace: {e.StackTrace}");
            }
        }
    }
}

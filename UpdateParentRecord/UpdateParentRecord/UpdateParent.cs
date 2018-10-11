using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
namespace UpdateParentRecord
{
    public class UpdateParent : IPlugin
    {
 
            public void Execute(IServiceProvider serviceProvider)
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = factory.CreateOrganizationService(context.UserId);
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                Entity obj = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));


                if (obj.Attributes.Contains("parentcustomerid"))
                {
                    EntityReference refAccount = (EntityReference)obj.Attributes["parentcustomerid"];
                    Entity objAccount = service.Retrieve(refAccount.LogicalName, refAccount.Id, new ColumnSet(true));

                    Entity contact = new Entity("contact");
                    contact.Id = context.PrimaryEntityId;

                    if (objAccount.Attributes.Contains("address1_line1"))
                        contact["address1_line1"] = objAccount["address1_line1"];

                    if (objAccount.Attributes.Contains("address1_line2"))
                        contact["address1_line2"] = objAccount["address1_line2"];

                    if (objAccount.Attributes.Contains("address1_line3"))
                        contact["address1_line3"] = objAccount["address1_line3"];

                    if (objAccount.Attributes.Contains("address1_stateorprovince"))
                        contact["address1_stateorprovince"] = objAccount["address1_stateorprovince"];

                    if (objAccount.Attributes.Contains("address1_stateorprovince"))
                        contact["address1_postalcode"] = objAccount["address1_stateorprovince"];

                    if (objAccount.Attributes.Contains("address1_country"))
                        contact["address1_country"] = objAccount["address1_country"];

                    service.Update(contact);
                }
                return;

            }
        }
    }



using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginexercise
{
    public class CreateUpdate  : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity obj = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));

            if(obj.Attributes.Contains("parentcustomerid"))
            {

                EntityReference refAccount = (EntityReference)obj.Attributes["parentcustomerid"];                
                Entity objAccount = service.Retrieve(refAccount.LogicalName, refAccount.Id, new ColumnSet(true));


                Entity contact = new Entity("contact");
                contact.Id = context.PrimaryEntityId;

                //EntityReference refContact = new EntityReference("contact", new Guid());

                //contact["customerid"] = refContac                    

                if (objAccount.Attributes.Contains("address1_line1"))
                    contact["address1_line1"] = objAccount["address1_line1"];
                else
                    contact["address1_line1"] = string.Empty;

                if (objAccount.Attributes.Contains("address1_city"))
                    contact["address1_city"] = objAccount["address1_city"];
                else
                    contact["address1_city"] = string.Empty;

                if (objAccount.Attributes.Contains("address1_line2"))
                    contact["address1_line2"] = objAccount["address1_line2"];
                else
                    contact["address1_line2"] = string.Empty;

                if (objAccount.Attributes.Contains("address1_line3"))
                    contact["address1_line3"] = objAccount["address1_line3"];
                else
                    contact["address1_line3"] = string.Empty;

                if (objAccount.Attributes.Contains("address1_stateorprovince"))
                    contact["address1_stateorprovince"] = objAccount["address1_stateorprovince"];
                else
                    contact["address1_stateorprovince"] = string.Empty;

                if (objAccount.Attributes.Contains("address1_stateorprovince"))
                    contact["address1_postalcode"] = objAccount["address1_stateorprovince"];
                else
                    contact["address1_postalcode"] = string.Empty;

                if (objAccount.Attributes.Contains("address1_country"))
                    contact["address1_country"] = objAccount["address1_country"];
                else
                    contact["address1_country"] = string.Empty;

                service.Update(contact);
            }           
        }
    }
}



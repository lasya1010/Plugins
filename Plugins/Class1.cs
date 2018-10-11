using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    public class Class1 : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
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

            Entity account = new Entity();
            account.LogicalName = "account";
            account.Id = context.PrimaryEntityId;
            account["telephone1"] = "753159";
            service.Update(account);


            //Guid guidCreatedAccount = Guid.Empty;
            //Entity accountToBeCreated = new Entity("account");
            //accountToBeCreated["name"] = "DBank";
            //guidCreatedAccount = service.Create(accountToBeCreated);
            //if (guidCreatedAccount != Guid.Empty)
            //    for (int i = 1; i <= 5; i++)
            //    {
            //        Entity eLetter = new Entity("letter");
            //        eLetter["subject"] = string.Format("Company Letter {0}", i.ToString());
            //        eLetter["regardingobjectid"] = new EntityReference("account", guidCreatedAccount);
            //        service.Create(eLetter);
            //    }

            //creating contact ,account sholud create
            //Entity account = new Entity("account");
            //account["primarycontactid"] = new EntityReference(obj.LogicalName, obj.Id);
            //service.Create(account);

            //Entity contact = new Entity("contact");
            //contact["mobilephone"] = "753159";
            //contact["emailaddress1"] = "test@test.com";
            //contact["yomifullname"] = new EntityReference(obj.LogicalName, obj.Id);
            //service.Update(contact);

        }
    }
}

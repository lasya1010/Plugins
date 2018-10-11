using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailValidationPlugin
{
    public class EmailValidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity obj = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));

            Entity update = new Entity();
            update.LogicalName = "account";
            update.Id = context.PrimaryEntityId;
            update["telephone1"] = "554545";
            update["websiteurl"] = "www.Test.com";
            update["customertypecode"] = new OptionSetValue(3);
            update["address1_city"] = "Ahmedabad";
            update["industrycode"] = new OptionSetValue(1);
            update["preferredcontactmethodcode"] = new OptionSetValue(2);
            update["donotfax"] = true;
            update["creditlimit"] = new Money(10000);

            service.Update(update);
        }
    }
}

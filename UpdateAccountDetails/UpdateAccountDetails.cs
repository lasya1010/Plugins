using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;




namespace UpdateAccountDetails
{
    public class UpdateAccountDetails : IPlugin

    {
         
            public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity obj = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));

            Entity account = new Entity();
            account.LogicalName = "account";
            account.Id = context.PrimaryEntityId;
            account["telephone1"] = "12346789";
            account["websiteurl"] = "www.hello.com";
            account["customertypecode"] = new OptionSetValue(2);
            //account["ADDRESS"] = "D/504  heights near sola bhagwat";
            account["preferredcontactmethodcode"] = new OptionSetValue(2);
            account["donotpostalmail"] = true;
            account["creditlimit"] = new Money(10000);
            account["industrycode"] = new OptionSetValue(1);
            account["ownershipcode"] = new OptionSetValue(2);

            service.Update(account);
        }
        


    
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AccountValidation
{
    public class AccountValidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

        
            Entity obj = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));
            if (obj.Attributes.Contains("name") && obj.Attributes.Contains("telephone1"))
            {

                QueryExpression query = new QueryExpression("account");
                query.ColumnSet = new ColumnSet("name", "telephone1");
                query.Criteria = new FilterExpression
                {
                    Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "name",
                                        Operator = ConditionOperator.Equal,
                                        Values = { obj["name"] }
                                    },
                                     new ConditionExpression
                                    {
                                        AttributeName = "telephone1",
                                        Operator = ConditionOperator.Equal,
                                        Values = { obj["telephone1"] }
                                    }
                                }
                };

                EntityCollection entities = service.RetrieveMultiple(query);
                if (entities != null && entities.Entities.Count > 1)
                {
                    throw new InvalidPluginExecutionException($"Record with same name and Telephone number already exists");
                }
                else
                {
                    // do nothing 
                }
            }
            else if(obj.Attributes.Contains("name") )
            {
                QueryExpression query = new QueryExpression("account");
                query.ColumnSet = new ColumnSet("name");
                query.Criteria = new FilterExpression
                {
                    Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "name",
                                        Operator = ConditionOperator.Equal,
                                        Values = { obj["name"] }
                                    }
                    }
                };

                EntityCollection entities = service.RetrieveMultiple(query);
                if (entities != null && entities.Entities.Count > 1)
                {
                    throw new InvalidPluginExecutionException($"Record with same Name already exists");
                }
                else
                {
                    // do nothing 
                }
            } 
      
        }
     }
}
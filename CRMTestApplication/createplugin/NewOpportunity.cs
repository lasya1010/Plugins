using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace SamplePlugins
{
    public class GenerateInvDetails : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName != "new_invoice")
                    return;

                decimal amount = ((Money)entity.Attributes["new_amount"]).Value;
                int terms = Convert.ToInt32(entity.Attributes["new_terms"]);
                int InvoiceDay = Convert.ToInt32(entity.Attributes["new_invoiceday"]);

                for (int i = 0; i < terms; i++)
                {
                    try
                    {
                        Entity InvoiceDetails = new Entity();
                        InvoiceDetails.LogicalName = "new_invoicedetails";
                        InvoiceDetails["new_installmentamount"] = new Money(amount / terms);
                        InvoiceDetails["new_invoicedate"] = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(i).Month, InvoiceDay);
                        service.Create(InvoiceDetails);
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
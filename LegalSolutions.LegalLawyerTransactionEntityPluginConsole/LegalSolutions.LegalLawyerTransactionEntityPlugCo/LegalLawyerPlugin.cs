using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LegalSolutions.LegalLawyerTransactionEntityPlugCo
{
    class LegalLawyerPluginProgram
    {
        static void Main(string[] args)
        {

            IOrganizationService service = GetCRMService("Url=https://devlegalsolution.crm4.dynamics.com/;UserName=hardik@avantit.no;Password=WdScbdQK12$;AuthType=Office365;");

            Entity obj = service.Retrieve("avantit_transaction", new Guid("45866855-D98F-E811-A967-000D3AB31F97"), new ColumnSet(true)); 

            EntityReference legalProject = new EntityReference();

            if (obj.Attributes.Contains("avantit_legalproject"))
                legalProject = (EntityReference)(obj.Attributes["avantit_legalproject"]);

            string transactionXML = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'> " +
                                     "<entity name='avantit_transaction'> " +
                                       "<attribute name='avantit_transactionid' /> " +
                                       "<attribute name='avantit_orderproduct' /> " +
                                       "<attribute name='avantit_legalproject' /> " +
                                       "<attribute name='avantit_erpordernumber' /> " +
                                       "<attribute name='avantit_amount' /> " +
                                       "<attribute name='statecode' /> " +
                                   "<order attribute='avantit_orderproduct' descending='false' /> " +
                               "<filter type='and'> " +
                                       "<condition attribute='avantit_legalproject' operator='eq' uitype='avantit_legalproject' value='{" + legalProject.Id + "}' /> " +
                               "</filter> " +
                               "<link-entity name='avantit_orderproduct' from='avantit_orderproductid' to='avantit_orderproduct' visible='false' link-type='outer' alias='orderproduct'> " +
                                   "<attribute name='avantit_producttype' alias='orderproduct_avantit_producttype' /> " +
                               "</link-entity> " +
                               "</entity>" +
                               "</fetch>";

            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(transactionXML));
            if(entityCollection.Entities.Count > 0)
            {
                var sumOpenTransaction   =  entityCollection.Entities
                                            .Where(t => ((OptionSetValue)t.Attributes["statecode"]).Value == 0
                                            && ((!t.Attributes.Contains("avantit_erpordernumber")) || (t.Attributes.Contains("avantit_erpordernumber") 
                                            && string.IsNullOrEmpty(Convert.ToString(t.Attributes["avantit_erpordernumber"])))))
                                            .Sum(t => Convert.ToDecimal(t.Attributes["avantit_amount"]));


                var sumDeductibleInvoiced = entityCollection.Entities
                                            .Where(t => t.Attributes.Contains("avantit_erpordernumber")
                                             && !string.IsNullOrEmpty(Convert.ToString(t.Attributes["avantit_erpordernumber"]))
                                             && t.Attributes.Contains("orderproduct_avantit_producttype")
                                             && ((OptionSetValue)(((AliasedValue)t.Attributes["orderproduct_avantit_producttype"]).Value)).Value == 174470001)
                                            .Sum(t => Convert.ToDecimal(t.Attributes["avantit_amount"]));

                var sumExpensesInvoiced = entityCollection.Entities
                                            .Where(t => t.Attributes.Contains("avantit_erpordernumber")
                                                    && !string.IsNullOrEmpty(Convert.ToString(t.Attributes["avantit_erpordernumber"]))
                                                    && t.Attributes.Contains("orderproduct_avantit_producttype")
                                                    && ((OptionSetValue)(((AliasedValue)t.Attributes["orderproduct_avantit_producttype"]).Value)).Value == 174470000)
                                            .Sum(t => Convert.ToDecimal(t.Attributes["avantit_amount"]));

                var sumPrepaymentInvoiced = entityCollection.Entities
                                            .Where(t => t.Attributes.Contains("avantit_erpordernumber")
                                                    && !string.IsNullOrEmpty(Convert.ToString(t.Attributes["avantit_erpordernumber"]))
                                                    && t.Attributes.Contains("orderproduct_avantit_producttype")
                                                    && ((OptionSetValue)(((AliasedValue)t.Attributes["orderproduct_avantit_producttype"]).Value)).Value == 174470002)
                                            .Sum(t => Convert.ToDecimal(t.Attributes["avantit_amount"]));
                
                Console.WriteLine(sumOpenTransaction);
                Console.WriteLine(sumExpensesInvoiced);
                Console.WriteLine(sumDeductibleInvoiced);
                Console.WriteLine(sumPrepaymentInvoiced);
            }

        }
        private static IOrganizationService GetCRMService(string crmURL)
        {
            try
            {
                // For Dynamics 365 Customer Engagement V9.X, set Security Protocol as TLS12
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var service = new CrmServiceClient(crmURL);
                return service;
            }
            catch (Exception e)
            {
                StringBuilder strErrorDetails = new StringBuilder();
                strErrorDetails.AppendFormat(" Organization URL: {0}", crmURL);

                if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
                    strErrorDetails.AppendLine(string.Format("InnerException: {0}", e.InnerException.Message));
                if (!string.IsNullOrEmpty(e.Message))
                    strErrorDetails.AppendLine(string.Format("Message: {0}", e.Message));
                if (!string.IsNullOrEmpty(e.StackTrace))
                    strErrorDetails.AppendLine(string.Format("StackTrace: {0}", e.StackTrace));

                throw new Exception($"Error occurred in GetCRMService. {strErrorDetails.ToString()}", e);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System.Net;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            IOrganizationService service = GetCRMService("Url=https://cutm1.crm8.dynamics.com/;UserName=140301csr008@cutm.ac.in;Password=L@l1th@1010;AuthType=Office365;");

            Entity obj = service.Retrieve("account", new Guid("F814107B-3F5B-E811-A95C-000D3AF245FB"), new ColumnSet(true));

            Entity update = new Entity();
            update.LogicalName = "account";
            update.Id = new Guid("F814107B-3F5B-E811-A95C-000D3AF245FB");
            update["telephone1"] = "554545";
            update["websiteurl"] = "www.Test.com";
            update["customertypecode"] = new OptionSetValue(3);
            update["address1_city"] = "Ahmedabad";
            update["industrycode"] = new OptionSetValue(1);
            update["preferredcontactmethodcode"] = new OptionSetValue(2);
            update["donotfax"] = true;
            update["creditlimit"] = new Money(10000);
            
            service.Update(update);

            //Entity con = new Entity("contact");
            //con["firstname"] = "Khushi";
            //con["lastname"] = "Jha";
            //con["mobilephone"] = "152365";
            //con["emailaddress1"] = "khushboo.jaa@test.com";            

            //Guid id = service.Create(con);

            //Entity test1 = new Entity("contact");
            //test1.Id = id;
            //test1["emailaddress1"] = "Test123@test.com"; 
            //service.Update(test1);

            //service.Delete("contact", id);



            //QueryExpression filterQueryExpression = new QueryExpression("account");
            //filterQueryExpression.ColumnSet = new ColumnSet(true);
            //filterQueryExpression.Criteria = new FilterExpression
            //{
            //    Conditions =
            //             {
            //                new ConditionExpression
            //                {
            //                    AttributeName = "statecode",
            //                    Operator = ConditionOperator.Equal,
            //                    Values = { 0 }  // Active Contacts
            //                },
            //            }
            //};

            //EntityCollection entities = service.RetrieveMultiple(filterQueryExpression);
            //if (entities != null && entities.Entities.Count > 0)
            //{
            //    Console.WriteLine("Account Count: " + entities.Entities.Count.ToString());
            //    foreach (Entity e in entities.Entities)
            //    {
            //        Console.WriteLine(e["name"]);
            //    }
            //}

            //QueryExpression query = new QueryExpression("contact");
            //query.ColumnSet = new ColumnSet(true);
            //query.Criteria = new FilterExpression
            //{
            //    Conditions =
            //             {
            //                new ConditionExpression
            //                {
            //                    AttributeName = "statecode",
            //                    Operator = ConditionOperator.Equal,
            //                    Values = { 0 }  // Active Contacts
            //                },
            //            }
            //};

            //EntityCollection entity = service.RetrieveMultiple(query);
            //if (entity != null && entity.Entities.Count > 0)
            //{
            //    Console.WriteLine("Contact Count: " + entity.Entities.Count.ToString());
            //    foreach (Entity e1 in entity.Entities)
            //    {
            //        Console.WriteLine(e1["lastname"]);
            //        Console.WriteLine(e1["firstname"]);
            //    }
            //}

            //QueryExpression queries = new QueryExpression("lead");
            //queries.ColumnSet = new ColumnSet(true);
            //queries.Criteria = new FilterExpression
            //{
            //    Conditions =
            //             {
            //                new ConditionExpression
            //                {
            //                    AttributeName = "statecode",
            //                    Operator = ConditionOperator.Equal,
            //                    Values = { 0 }  
            //                },
            //            }
            //};

            //EntityCollection multi = service.RetrieveMultiple(queries);
            //if (multi != null && multi.Entities.Count > 0)
            //{
            //    Console.WriteLine("Lead Count: " + multi.Entities.Count.ToString());
            //    foreach (Entity e2 in multi.Entities)
            //    {
            //        Console.WriteLine(e2["subject"]);
            //    }
            //}


        }
        public static IOrganizationService GetCRMService(string crmURL)
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

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRMTestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            IOrganizationService service = GetCRMService("Url=https://cutm2.crm8.dynamics.com/;UserName=140301csr008@cutm.ac.in;Password=L@l1th@1010;AuthType=Office365;");


            Entity obj = service.Retrieve("contact", new Guid("DB963F10B368E811A956000D3AF04F8C"), new ColumnSet(true));

            if (obj.Attributes.Contains("parentcustomerid"))
            {
                EntityReference refAccount = (EntityReference)obj.Attributes["parentcustomerid"];
                Entity objAccount = service.Retrieve(refAccount.LogicalName, refAccount.Id, new ColumnSet(true));

                Entity contact = new Entity("contact");
                contact.Id = new Guid("EF88C9EB-BB59-E811-A953-000D3AF28740");

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

           
        //            Console.WriteLine(e["fullname"]);
        //            // EntityReference
        //            // OptionSet
                    
        //            if (e.Attributes.Contains("telephone1"))
        //            { return;
        //    //service.Create("contact")
        //    Entity test = new Entity("contact");
        //    test["firstname"] = "Test";
        //    test["lastname"] = "Contact2";
        //    test["emailaddress1"] = "Test2@test.com";
        //    Guid id = service.Create(test);

        //    //service.Update("contact")
        //    Entity test1 = new Entity("contact");
        //    test1.Id = id;
        //    test1["emailaddress1"] = "Test123@test.com";
        //    service.Update(test1);

        //    //service.Delete("contact")

        //    Entity e1 = service.Retrieve("contact", id, new ColumnSet(true));

        //    QueryExpression filterQueryExpression = new QueryExpression("contact");
        //    filterQueryExpression.ColumnSet = new ColumnSet(true);
        //    filterQueryExpression.Criteria = new FilterExpression
        //    {
        //        Conditions =
        //                {
        //                    new ConditionExpression
        //                    {
        //                        AttributeName = "statecode",
        //                        Operator = ConditionOperator.Equal,
        //                        Values = { 0 }  // Active Contacts
        //                    },                            
        //                }
        //    };


        //    EntityCollection entities = service.RetrieveMultiple(filterQueryExpression);

        //    if(entities != null && entities.Entities.Count > 0)
        //    {
        //        foreach(Entity e in entities.Entities)
        //        {

        //            }
        //        }
        //    }

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

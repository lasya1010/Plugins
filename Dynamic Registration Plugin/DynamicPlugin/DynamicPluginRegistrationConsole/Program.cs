using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicPluginRegistrationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IOrganizationService service = GetCRMService("Url=https://cutm.crm8.dynamics.com/;UserName=140301csr008@cutm.ac.in;Password=L@l1th@2020;AuthType=Office365;");

            Entity assembly = service.Retrieve("lg_entityschemaname", new Guid("00662A609595E811A95D000D3AF04FC2"), new ColumnSet(true));
            string entityName = "";
            if (assembly.Attributes.Contains("lg_name"))
                entityName = Convert.ToString(assembly.Attributes["lg_name"]);
           
            CreateDeleteDynamicPluginSteps(service, entityName, "Create");
            CreateDeleteDynamicPluginSteps(service, entityName, "Update");
            CreateDeleteDynamicPluginSteps(service, entityName, "Delete");

        }
        public static bool CreateDeleteDynamicPluginSteps(IOrganizationService service, string entityName, string messageName, bool isCreate = true)
        {
            var assemblyName = "DynamicPlugin";
            var pluginTypeName = "DynamicPlugin.DynamicPluginclass";
            Guid pluginId = GetPluginTypeId(service, assemblyName, pluginTypeName);
            Guid messageId = GetMessageId(service, messageName.ToString());
            Guid sdkMessageFilterId = GetSdkMessageFilterId(service, entityName, messageId);
            if (isCreate)
            {
                try
                {
                    var existingStep = IsExistingStep(service, sdkMessageFilterId, pluginId, messageId);
                    if (!existingStep)
                    {
                        var step = new Entity("sdkmessageprocessingstep");
                        step["mode"] = new OptionSetValue(0); //  Asynchronous = 1, Synchronous = 0
                        step["name"] = string.Format("{0}: {1} of {2}", pluginTypeName, messageName, entityName);
                        step["description"] = string.Format("{0}: {1} of {2}", pluginTypeName, messageName, entityName);
                        step["rank"] = 1;

                        if (messageName == "Delete")
                            step["stage"] = new OptionSetValue(10);  //PreValidation = 10, PreOperation = 20, PostOperation = 40
                        else
                            step["stage"] = new OptionSetValue(40);  //PreValidation = 10, PreOperation = 20, PostOperation = 40

                        step["supporteddeployment"] = new OptionSetValue(0); // ServerOnly = 0
                        step["invocationsource"] = new OptionSetValue(0); // 0 -> Parent, 1 -> Child                    
                        step["plugintypeid"] = new EntityReference("plugintype", pluginId);
                        step["sdkmessageid"] = new EntityReference("sdkmessage", messageId);

                        if (sdkMessageFilterId != Guid.Empty)
                            step["sdkmessagefilterid"] = new EntityReference("sdkmessagefilter", sdkMessageFilterId);

                        step["configuration"] = "a5fd0de7-2396-e811-8145-5065f38aea61"; // Add Service EndPoint Id

                        service.Create(step);
                       
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                // do nothing here for understanding 
            }

            return true;
        }
        public static Guid GetSdkMessageFilterId(IOrganizationService service, string entityName, Guid messageId)
        {
            QueryExpression sdkMessageFilterQueryExpression = new QueryExpression("sdkmessagefilter");
            sdkMessageFilterQueryExpression.ColumnSet = new ColumnSet("sdkmessagefilterid");
            sdkMessageFilterQueryExpression.Criteria = new FilterExpression
            {
                Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "primaryobjecttypecode",
                                Operator = ConditionOperator.Equal,
                                Values = { entityName }
                            },
                            new ConditionExpression
                            {
                                AttributeName = "sdkmessageid",
                                Operator = ConditionOperator.Equal,
                                Values = { messageId }
                            },
                        }
            };

            EntityCollection sdkMessageFilters = service.RetrieveMultiple(sdkMessageFilterQueryExpression);
            if (sdkMessageFilters.Entities.Count != 0)
            {
                return sdkMessageFilters.Entities.First().Id;
            }
            else
                return Guid.Empty;
        }
        public static Guid GetMessageId(IOrganizationService service, string messageName)
        {
            QueryExpression sdkMessageQueryExpression = new QueryExpression("sdkmessage");
            sdkMessageQueryExpression.ColumnSet = new ColumnSet("sdkmessageid");
            sdkMessageQueryExpression.Criteria = new FilterExpression
            {
                Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "name",
                                Operator = ConditionOperator.Equal,
                                Values = { messageName }
                            },
                        }
            };

            EntityCollection sdkMessages = service.RetrieveMultiple(sdkMessageQueryExpression);
            if (sdkMessages.Entities.Count != 0)
            {
                return sdkMessages.Entities.First().Id;
            }

            return Guid.Empty;
        }
        public static Guid GetPluginTypeId(IOrganizationService service, string assemblyName, string pluginTypeName)
        {
            QueryExpression pluginAssemblyQueryExpression = new QueryExpression("pluginassembly");
            pluginAssemblyQueryExpression.ColumnSet = new ColumnSet("pluginassemblyid");
            pluginAssemblyQueryExpression.Criteria = new FilterExpression
            {
                Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "name",
                                Operator = ConditionOperator.Equal,
                                Values = { assemblyName }
                            },
                        }
            };
            EntityCollection pluginAssemblies = service.RetrieveMultiple(pluginAssemblyQueryExpression);
            Guid assemblyId = Guid.Empty;
            if (pluginAssemblies.Entities.Count != 0)
            {
                assemblyId = pluginAssemblies.Entities.First().Id;
                QueryExpression pluginTypeQueryExpression = new QueryExpression("plugintype");
                pluginTypeQueryExpression.ColumnSet = new ColumnSet("plugintypeid");
                pluginTypeQueryExpression.Criteria = new FilterExpression
                {
                    Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "pluginassemblyid",
                                Operator = ConditionOperator.Equal,
                                Values = { assemblyId }
                            },
                            new ConditionExpression
                            {
                                AttributeName = "typename",
                                Operator = ConditionOperator.Equal,
                                Values = { pluginTypeName }
                            },
                        }
                };

                EntityCollection pluginTypes = service.RetrieveMultiple(pluginTypeQueryExpression);
                if (pluginTypes.Entities.Count != 0)
                {
                    return pluginTypes.Entities.First().Id;
                }
            }

            return Guid.Empty;
        }
        public static bool IsExistingStep(IOrganizationService service, Guid messageFilterId, Guid pluginId, Guid messageId)
        {
            var result = GetAllStepsFor(service, messageFilterId, pluginId, messageId);
            if (result == null || result.Entities.Count == 0)
            {
                return false;
            }
            return true;
        }
        public static EntityCollection GetAllStepsFor(IOrganizationService service, Guid messageFilterId, Guid pluginId, Guid messageId, bool deleteAll = false)
        {
            var query = new QueryExpression("sdkmessageprocessingstep");
            query.Criteria.Conditions.Add(new ConditionExpression("sdkmessagefilterid", ConditionOperator.Equal, messageFilterId));
            query.Criteria.Conditions.Add(new ConditionExpression("sdkmessageid", ConditionOperator.Equal, messageId));
            query.Criteria.Conditions.Add(new ConditionExpression("plugintypeid", ConditionOperator.Equal, pluginId));

            if (!deleteAll)
                query.Criteria.Conditions.Add(new ConditionExpression("filteringattributes", ConditionOperator.NotEqual, "ownerid"));

            var result = service.RetrieveMultiple(query);
            return result;
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

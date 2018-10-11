using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace AvantIT.DynamicStepRegistration
{
    public class Helper
    {
        public static bool DuplicateRecordChecking(IOrganizationService service, string entityName, string fieldName, string fieldValue)
        {
            QueryExpression queryExpression = new QueryExpression(entityName);
            queryExpression.ColumnSet = new ColumnSet(true);
            queryExpression.Criteria = new FilterExpression
            {
                Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = fieldName,
                                Operator = ConditionOperator.Equal,
                                Values = { fieldValue }
                            }
                        }
            };

            EntityCollection duplicateEntities = service.RetrieveMultiple(queryExpression);
            if (duplicateEntities.Entities != null && duplicateEntities.Entities.Count > 1)
                return true; // Duplicate record exists
            else
                return false; // No Duplicate record found
        }

        public static bool DuplicateSharePointSettingRecordChecking(IOrganizationService service, string entityName, string fieldName, string fieldValue, string messageName)
        {
            QueryExpression queryExpression = new QueryExpression(entityName);
            queryExpression.ColumnSet = new ColumnSet(true);
            queryExpression.Criteria = new FilterExpression
            {
                Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = fieldName,
                                Operator = ConditionOperator.Equal,
                                Values = { fieldValue }
                            },
                            new ConditionExpression
                            {
                                AttributeName = "avant_parentid",
                                Operator = ConditionOperator.Null
                            }
                        },
                FilterOperator = LogicalOperator.And
            };

            EntityCollection DuplicateEntities = service.RetrieveMultiple(queryExpression);

            if (DuplicateEntities.Entities != null && ((DuplicateEntities.Entities.Count > 1 && messageName == "Create") || (DuplicateEntities.Entities.Count > 2 && messageName == "Update")))
                return true; // Duplicate record exists
            else
                return false; // No Duplicate record found
        }

        public static bool IsFieldExist(IOrganizationService service, String entityName, String fieldName)
        {
            RetrieveEntityRequest request = new RetrieveEntityRequest
            {
                EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Attributes,
                LogicalName = entityName
            };
            RetrieveEntityResponse response = (RetrieveEntityResponse)service.Execute(request);
            return response.EntityMetadata.Attributes.FirstOrDefault(element => element.LogicalName == fieldName) != null;
        }

        public static bool IsEntityExist(IOrganizationService service, string entityLogicalName)
        {
            try
            {
                RetrieveEntityRequest req = new RetrieveEntityRequest()
                {
                    LogicalName = entityLogicalName,

                };
                RetrieveEntityResponse resp = service.Execute(req) as RetrieveEntityResponse;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool CreateDeleteDynamicPluginSteps(IOrganizationService service, string entityName, string messageName, string filteringAttributes, bool isCreate, bool deleteAll = false)
        {
            var assemblyName = "AvantIT.SendIntegrationDataToAzure";
            var pluginTypeName = "AvantIT.SendIntegrationDataToAzure.SendIntegrationData";
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

                        if (!string.IsNullOrEmpty(filteringAttributes))
                            step["filteringattributes"] = filteringAttributes;

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
                // Delete the steps
                DeleteAllRegisteredStepsFor(service, sdkMessageFilterId, pluginId, messageId, deleteAll);
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

        public static void DeleteAllRegisteredStepsFor(IOrganizationService service, Guid messageFilterId, Guid pluginId, Guid messageId, bool deleteAll)
        {
            var result = GetAllStepsFor(service, messageFilterId, pluginId, messageId, deleteAll);

            foreach (var step in result.Entities)
            {
                service.Delete("sdkmessageprocessingstep", step.Id);
            }
        }
    }
}

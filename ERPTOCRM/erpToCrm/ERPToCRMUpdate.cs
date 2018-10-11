using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using ERPToCRM.JsonResult;
using AzureHelper;
using CRMHelper;
using Microsoft.Xrm.Sdk;
using System.Configuration;

namespace ErpToCrm
{
    public static class ERPToCRMUpdate
    {
        [FunctionName("ERPToCRMUpdate")]
        public static void Run([ServiceBusTrigger("%ErptoCrmQueueName%", AccessRights.Manage, Connection = "ServiceBusConnection")]string queueMsg, ILogger log)
        {
            if (!string.IsNullOrEmpty(queueMsg))
            {
                BindData(queueMsg, log);
            }
            else
            {
                WriteInfoLog(log, "Queue message is blank.");
            }
        }
        public static bool BindData(string queueMsg, ILogger log = null)
        {
            JsonResult jObject = JsonConvert.DeserializeObject<JsonResult>(queueMsg);
            string crmURL = string.Empty;
            try
            {
                crmURL = AzureManager.GetValueFromAzureVault(ConfigurationManager.AppSettings["KeyVaultUrl"] + jObject.OrgUniqueName);
            }
            catch (Exception ex)
            {
                WriteInfoLog(log, "Error in Queue message." + ex.ToString());
            }

            if (!string.IsNullOrEmpty(crmURL))
            {
                var service = CRMManager.GetCRMService(crmURL);

                Entity entityName = new Entity("ait_msteams");
                entityName.Id = new Guid(jObject.RecordId);
                entityName["ait_teamsid"] = jObject.TeamId;
                entityName["ait_groupid"] = jObject.GroupId;
                entityName["ait_teamslibraryurl"] = jObject.TeamLibraryUrl;
                service.Update(entityName);
                return true;
            }

            return false;
        }
        private static void WriteInfoLog(ILogger log, string message)
        {
            if (log != null)
                log.LogInformation(message);
        }
    }
}

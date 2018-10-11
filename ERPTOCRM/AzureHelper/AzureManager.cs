using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Text;

namespace AzureHelper
{
    public class AzureManager
    {
        public static string GetValueFromAzureVault(string vaultBaseURL)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            string value = string.Empty;
            try
            {
                var keyVault = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                value = keyVault.GetSecretAsync(vaultBaseURL).Result.Value;
            }
            catch (Exception ex)
            {
                StringBuilder strErrorDetails = new StringBuilder();
                strErrorDetails.AppendFormat(" Valut Base URL: {0}", vaultBaseURL);

                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                    strErrorDetails.AppendFormat(" ,InnerException: {0}", ex.InnerException.Message);
                if (!string.IsNullOrEmpty(ex.Message))
                    strErrorDetails.AppendFormat(" ,Message: {0}", ex.Message);
                if (!string.IsNullOrEmpty(ex.StackTrace))
                    strErrorDetails.AppendFormat(" ,StackTrace: {0}", ex.StackTrace);

                throw new Exception($"Error occurred in GetValueFromAzureVault. {strErrorDetails.ToString()}", ex);
            }

            return value;
        }
    }
}

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Text.RegularExpressions;

namespace EmailValidation
{
    public class emailValidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity account = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));

            string email = string.Empty;
            if (account.Attributes.Contains("emailaddress1"))
            {
                email = account.Attributes["emailaddress1"].ToString();
                if (!ValidateEmail(email))
                {
                    throw new InvalidPluginExecutionException("Invalid Email Address");
                }
            };
        }
        private static bool ValidateEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
                return true;
            return false;
        }
    }
}

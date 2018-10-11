using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendDataToAzure
{
    public class SharePointSharing
    {
        public string FolderName { get; set; }
        public string Permission { get; set; }
        public string Operation { get; set; }
        public string ExternalUserEmail { get; set; }
        public string Url { get; set; }
        public string OrgUniqueName { get; set; }

    }
}

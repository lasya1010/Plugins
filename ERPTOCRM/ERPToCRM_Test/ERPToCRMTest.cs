using NUnit.Framework;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ERPToCRM_Test
{
    [TestFixture]
    public class ERPToCRMTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test, Order(1)]

        public void BindERPToCRMTest()
        {
            string jsonString = GetSampleJsonString();
            var result = ErpToCrm.ErpToCrm.BindData(jsonString, null);
            Assert.IsTrue(result);
            var fakedContext = new XrmFakedContext();
            fakedContext.ProxyTypesAssembly = Assembly.GetExecutingAssembly();

            var contact1 = new Entity("contact") { Id = Guid.NewGuid() }; contact1["fullname"] = "Contact 1"; contact1["firstname"] = "First 1";
            var contact2 = new Entity("contact") { Id = Guid.NewGuid() }; contact2["fullname"] = "Contact 2"; contact2["firstname"] = "First 2";

            fakedContext.Initialize(new List<Entity>() { contact1, contact2 });

            var guid = Guid.NewGuid();

            //Empty contecxt (no Initialize), but we should be able to query any typed entity without an entity not found exception

            var service = fakedContext.GetFakedOrganizationService();

           
        }

        private string GetSampleJsonString()
        {
            var jsonString = "{" + "'OrgUniqueName': 'org1bc6c1a4'," +
                                  "'PrimaryField': 'sic'," +
                                  "'PrimaryValue': '3895006'," +
                                  "'CodeField': 'accountnumber'," +
                                  "'CodeValue': '10027'," +
                                  "'EntityName': 'account'," +
                                  "'RecordId': '98ad64d8-e7a9-e811-814b-5065f38b06f1'" +
                             "}'";

            return jsonString;
        }
    }
}

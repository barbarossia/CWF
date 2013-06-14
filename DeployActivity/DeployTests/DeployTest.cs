using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Activities;
using DeployActivity;

namespace DeployTests
{
    // This is basically a functional end-to-end test which is dependent upon data already being set up in
    // the Dev database. It is not suitable for a unit test because it is fragile (dependent upon DB state).
    [TestClass]
    public class DeployTest
    {
        [TestMethod]
        public void TestDeployWithDependencies()
        {
            try
            {
                File.Delete(@"d:\tmp\bin\ActivityLibrary1.dll");
                File.Delete(@"d:\tmp\bin\ActivityLibrary2.dll");
                File.Delete(@"d:\tmp\WorkflowWithIndirectDependency.xamlx");
            }
            catch { }

            var act = new DeployToIIS { FilePath = @"d:\tmp", ActivityName = "Microsoft.Support.Workflow.Activity.WorkflowWithIndirectDependency", ActivityVersion = "1.0.0.0" };
            var wf = new WorkflowInvoker(act);
            wf.Extensions.Add(new QueryServiceURIExtension(@"http://pqocwfddb01/WorkflowQueryServiceDEV/WorkflowsQueryService.svc"));
            wf.Invoke();
            Assert.IsTrue(File.Exists(@"d:\tmp\bin\ActivityLibrary1.dll"));
            Assert.IsTrue(File.Exists(@"d:\tmp\bin\ActivityLibrary2.dll"));
            Assert.IsTrue(File.Exists(@"d:\tmp\WorkflowWithIndirectDependency.xamlx"));

        }
    }
}

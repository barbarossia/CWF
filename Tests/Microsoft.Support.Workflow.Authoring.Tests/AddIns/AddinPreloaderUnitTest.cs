using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;

namespace Microsoft.Support.Workflow.Authoring.Tests.AddIns
{
    [TestClass]
    public class AddinPreloaderUnitTest
    {

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void AddinPreloader_TestGet()
        {
            AddinPreloader current = AddinPreloader.Current;
            using (var intel = new ImplementationOfType(typeof(ExpressionEditorHelper)))
            {
                bool IsCreateIntellisenseList = false;
                intel.Register(() => ExpressionEditorHelper.CreateIntellisenseList())
                    .Execute(() =>
                    {
                        IsCreateIntellisenseList = true;
                        return new TreeNode();
                    });

                using (var appDomain = new ImplementationOfType(typeof(AppDomain)))
                {
                    appDomain.Register(() => AppDomain.CreateDomain(Argument<string>.Any, null, Argument<AppDomainSetup>.Any))
                    .Return(AppDomain.CurrentDomain);
                    
                    PrivateObject po = new PrivateObject(current);
                    IDesignerContract ic = new DesignerAddIn();
                    Dictionary<IDesignerContract, AppDomain> list = po.GetFieldOrProperty("addinDomainMaps") as Dictionary<IDesignerContract, AppDomain>;
                    list.Add(ic, AppDomain.CurrentDomain);
                    po.SetFieldOrProperty("addin", ic);
                    po.SetFieldOrProperty("addinDomainMaps", list);

                    IDesignerContract designerAddin = current.Get();
                    Assert.IsTrue(IsCreateIntellisenseList);
                }
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void AddinPreloader_TestUnload()
        {
            AddinPreloader current = AddinPreloader.Current;
            using (var intel = new ImplementationOfType(typeof(ExpressionEditorHelper)))
            {
                bool IsCreateIntellisenseList = false;
                intel.Register(() => ExpressionEditorHelper.CreateIntellisenseList())
                    .Execute(() =>
                    {
                        IsCreateIntellisenseList = true;
                        return new TreeNode();
                    });

                using (var appDomain = new ImplementationOfType(typeof(AppDomain)))
                {
                    bool isUnload = false;
                    appDomain.Register(() => AppDomain.CreateDomain(Argument<string>.Any, null, Argument<AppDomainSetup>.Any))
                    .Return(AppDomain.CurrentDomain);
                    appDomain.Register(() => AppDomain.Unload(AppDomain.CurrentDomain)).Execute(() => { isUnload = true; });

                    PrivateObject po = new PrivateObject(current);
                    IDesignerContract ic = new DesignerAddIn();
                    Dictionary<IDesignerContract, AppDomain> list = po.GetFieldOrProperty("addinDomainMaps") as Dictionary<IDesignerContract, AppDomain>;
                    list.Add(ic, AppDomain.CurrentDomain);
                    po.SetFieldOrProperty("addin", ic);
                    po.SetFieldOrProperty("addinDomainMaps", list);

                    current.Unload(ic);
                    //Assert.IsTrue(list.Count == 0);
                    Assert.IsTrue(IsCreateIntellisenseList);
                    Assert.IsTrue(isUnload);
                }
            }
        }
    }
}

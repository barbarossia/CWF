using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xaml;
using AuthoringToolTests.Services;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.ExpressionEditor
{
    [TestClass]
    public class ExpressionEditorServiceUnitTest
    {
        [WorkItem(321729)]
        [TestMethod]
        [Description("Check if CreateExpressionEditor works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void aaExpressionEditor_TestCreateExpressionEditor()
        {
            using (new CachingIsolator())
            {
                AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                    .Where(a => a.Name == "mscorlib").Take(1).ToArray();
                ExpressionEditorHelper.GetReferencesFunc = () => { return names; };
                using (var assembly = new Implementation<Assembly>())
                {
                    using (var assemblyMock = new ImplementationOfType(typeof(Assembly)))
                    {
                        assemblyMock.Register(() => Assembly.GetExecutingAssembly()).Return(assembly.Instance);
                        AssemblyCompanyAttribute attrib = new AssemblyCompanyAttribute("TestCompany");
                        using (var attribMock = new ImplementationOfType(typeof(Attribute)))
                        {
                            attribMock.Register(() => Attribute.GetCustomAttribute(assembly.Instance, typeof(AssemblyCompanyAttribute), false))
                                .Return(attrib);

                            Caching.CacheAssembly(new List<ActivityAssemblyItem>() { TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1 });
                            ExpressionEditorHelper.ClearIntellisenseList();
                            ExpressionEditorHelper.CreateIntellisenseList();

                            Caching.CacheAssembly(new List<ActivityAssemblyItem>() { TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib2 });
                            EditingContext ec = new EditingContext();
                            ModelTreeManager mtm = new ModelTreeManager(ec);
                            IExpressionEditorInstance result;
                            ExpressionEditorService service = new ExpressionEditorService();
                            service.IntellisenseNode = ExpressionEditorHelper.CreateIntellisenseList();
                            TreeNode newNode = service.IntellisenseNode.Nodes.SingleOrDefault(n => n.Name == "New");
                            Assert.IsNotNull(newNode);
                            Assert.AreEqual(TreeNodeType.Primitive, newNode.ItemType);

                            service.UpdateContext(null, null);

                            object obj = new object();
                            mtm.Load(obj);
                            result = service.CreateExpressionEditor(
                                null, null, new List<ModelItem> { mtm.Root }, string.Empty);
                            Assert.IsNotNull(result);

                            Variable<string> argument = new Variable<string>("arg1");
                            mtm.Load(argument);
                            result = service.CreateExpressionEditor(
                                null, null, new List<ModelItem> { mtm.Root }, string.Empty, Size.Empty);
                            Assert.IsNotNull(result);

                            result = service.CreateExpressionEditor(
                                null, null, new List<ModelItem> { mtm.Root }, string.Empty, typeof(Sequence));
                            Assert.IsNotNull(result);

                            result = service.CreateExpressionEditor(
                                null, null, new List<ModelItem> { mtm.Root }, string.Empty, typeof(Sequence), Size.Empty);
                            Assert.IsNotNull(result);

                            service.CloseExpressionEditors();
                        }
                    }
                }
            }
        }
    }
}

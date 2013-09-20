using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AuthoringToolTests.Services;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Support.Workflow.Authoring.Tests.ExpressionEditor
{
    [TestClass]
    public class TreeNodeUnitTest
    {
        [WorkItem(322360)]
        [TestMethod]
        [Description("Check if TreeNode works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void aaTreeNode_TestTreeNode()
        {
            using (new CachingIsolator())
            {
                AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                    .Where(a => a.Name == "mscorlib" && a.Version.Major == 4).ToArray();
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

                            ExpressionEditorHelper.ClearIntellisenseList();
                            TreeNode node = ExpressionEditorHelper.CreateIntellisenseList();
                            Assert.AreEqual(string.Empty, node.GetFullPath());
                    
                            TreeNode stringNode = node.Nodes
                                .Single(n => n.Name == "System").Nodes
                                .Single(n => n.Name == "String");
                            Assert.AreEqual("System.String", stringNode.GetFullPath());
                            Assert.AreEqual(stringNode.Name, stringNode.ToString());
                            Assert.IsTrue(stringNode.IsMatch("Str"));
                            Assert.IsFalse(stringNode.IsMatch("Stra"));
                            Assert.AreEqual(1, stringNode.CompareTo(null));
                            Assert.IsFalse(stringNode.HasOverrideMethods);
                            Assert.IsNull(stringNode.MethodInfoes);
                          
                            TreeNode cloneNode = stringNode.Nodes.Single(n => n.Name == "Clone");
                            Assert.AreEqual("System.String.Clone", cloneNode.GetFullPath());
                            Assert.IsFalse(cloneNode.HasOverrideMethods);
                            Assert.AreEqual(1, cloneNode.MethodInfoes.Length);
                            Assert.AreEqual(cloneNode.MethodInfoes.Single(), cloneNode.Description);
                 
                            TreeNode substringNode = stringNode.Nodes.Single(n => n.Name == "Substring");
                            Assert.AreEqual("System.String.Substring", substringNode.GetFullPath());
                            Assert.IsTrue(substringNode.HasOverrideMethods);
                            Assert.AreEqual(2, substringNode.MethodInfoes.Length);
                            Assert.AreEqual(substringNode.GetMethodDescriptionAt(0), substringNode.Description);
                            Assert.AreEqual(
                                substringNode.GetMethodDescriptionAt(substringNode.MethodInfoes.Length - 1),
                                substringNode.GetMethodDescriptionAt(-1)
                            );
                        }
                    }
                }
            }
        }
    }
}
